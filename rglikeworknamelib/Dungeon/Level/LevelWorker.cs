using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Dungeon.Level {
    public class LevelWorker {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<Point, GameLevel> onGeneration_;
        private readonly Dictionary<Point, GameLevel> onLoad_;
        private readonly Dictionary<Point, MapSector> onSave_;
        public Dictionary<Point, MapSector> Ready;
        private bool exit;
        private Point lastTry;

        public LevelWorker() {
            onLoad_ = new Dictionary<Point, GameLevel>();
            onGeneration_ = new Dictionary<Point, GameLevel>();
            onSave_ = new Dictionary<Point, MapSector>();
            Ready = new Dictionary<Point, MapSector>();
        }

        public bool Loading() {
            return onLoad_.Count > 0;
        }

        public bool Saving() {
            return onSave_.Count > 0;
        }

        public bool Generating() {
            return onGeneration_.Count > 0;
        }

        public int LoadCount() {
            return onLoad_.Count;
        }

        public int GenerationCount() {
            return onGeneration_.Count;
        }

        public int SaveCount() {
            return onSave_.Count;
        }

        public void Run() {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            Thread.CurrentThread.IsBackground = true;
            while (!exit) {
                if (onSave_.Count > 0) {
                    KeyValuePair<Point, MapSector> t = onSave_.First();
                    SaveSector(t.Value);
                    onSave_.Remove(t.Key);
                }

                if (onLoad_.Count > 0) {
                    KeyValuePair<Point, GameLevel> t = onLoad_.First();
                    if (Ready.ContainsKey(t.Key)) {
                        Ready.Remove(t.Key);
                    }
                    Ready.Add(t.Key, LoadSector(t.Key.X, t.Key.Y, t.Value));
                    //gl.GetSector(t.Item1.X, t.Item1.Y);
                    onLoad_.Remove(t.Key);
                }

                if (onGeneration_.Count > 0) {
                    KeyValuePair<Point, GameLevel> tt = onGeneration_.First();
                    var ms = new MapSector(tt.Value, tt.Key.X, tt.Key.Y);
                    ms.Rebuild(tt.Value.MapSeed);
                    if (Ready.ContainsKey(tt.Key)) {
                        Ready.Remove(tt.Key);
                    }
                    Ready.Add(tt.Key, ms);
                    //gl.GetSector(tt.Item1.X, tt.Item1.Y);
                    onGeneration_.Remove(tt.Key);
                }

                lastTry = new Point(-999, -999);

                if (!Loading() && !Saving() && !Generating()) {
                    Thread.Sleep(300);
                }
            }
        }

        public void Generate(Point p, GameLevel gl) {
            if (!onGeneration_.ContainsKey(p)) {
                onGeneration_.Add(p, gl);
            }
        }

        public void Load(Point p, GameLevel gl) {
            if (!onLoad_.ContainsKey(p)) {
                onLoad_.Add(p, gl);
            }
        }

        public void Save(MapSector ms) {
            var p = new Point(ms.SectorOffsetX, ms.SectorOffsetY);
            if (onSave_.ContainsKey(p)) {
                onSave_.Remove(p);
            }
            onSave_.Add(p, ms);
        }

        public MapSector TryGet(Point p, GameLevel gl) {
            if (lastTry == p) {
                return null;
            }
            if (Ready.ContainsKey(p)) {
                return Ready[p];
            }

            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", p.X, p.Y))) {
                Load(p, gl);
            }
            else {
                Generate(p, gl);
            }

            lastTry = p;

            return null;
        }

        /// <summary>
        ///     only for all map save. Remake!
        /// </summary>
        /// <param name="a"></param>
        internal void SaveSector(MapSector a) {
            try {
                var binaryFormatter = new BinaryFormatter();

                var fileStream =
                    new FileStream(
                        Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", a.SectorOffsetX, a.SectorOffsetY),
                        FileMode.Create);
                var gZipStream = new GZipStream(fileStream, CompressionMode.Compress);
                binaryFormatter.Serialize(gZipStream, a.SectorOffsetX);
                binaryFormatter.Serialize(gZipStream, a.SectorOffsetY);
                binaryFormatter.Serialize(gZipStream, a.Blocks);
                binaryFormatter.Serialize(gZipStream, a.Floors);
                binaryFormatter.Serialize(gZipStream, a.Biom);
                binaryFormatter.Serialize(gZipStream, a.Creatures);
                binaryFormatter.Serialize(gZipStream, a.Decals);
                gZipStream.Close();
                gZipStream.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            }
            catch (Exception e) {
                logger.Error("SAVE ERROR -- " + e);
            }
        }

        private MapSector LoadSector(int sectorOffsetX, int sectorOffsetY, GameLevel gl) {
            try {
                if (
                    File.Exists(Settings.GetWorldsDirectory() +
                                string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY))) {
                    var binaryFormatter = new BinaryFormatter();

                    var fileStream = new FileStream(
                        Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY),
                        FileMode.Open);

                    var gZipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                    object q1 = binaryFormatter.Deserialize(gZipStream); //SectorOffsetX
                    object q2 = binaryFormatter.Deserialize(gZipStream); //SectorOffsetY
                    object q3 = binaryFormatter.Deserialize(gZipStream); //Blocks
                    object q4 = binaryFormatter.Deserialize(gZipStream); //Floors
                    object q5 = binaryFormatter.Deserialize(gZipStream); //Biom
                    object q6 = binaryFormatter.Deserialize(gZipStream); //Creatures
                    object q7 = binaryFormatter.Deserialize(gZipStream); //Decals
                    gZipStream.Close();
                    gZipStream.Dispose();
                    fileStream.Close();
                    fileStream.Dispose();
                    var t = new MapSector(gl, q1, q2, q3, q4, q5, q6, q7);
                    foreach (IBlock block in t.Blocks) {
                        ((Block) block).OnLoad();
                        if (block.Data.Prototype == typeof (StorageBlock)) {
                            foreach (IItem item in ((StorageBlock) block).StoredItems) {
                                item.OnLoad();
                            }
                        }
                    }
                    t.Ready = true;
                    return t;
                }
                return null;
            }
            catch (Exception e) {
                logger.Error("LOAD ERROR -- " + e);
                var t = new MapSector(gl, sectorOffsetX, sectorOffsetY);
                t.Rebuild(gl.MapSeed);
                return t;
            }
        }

        public void Stop() {
            exit = true;
        }
    }
}