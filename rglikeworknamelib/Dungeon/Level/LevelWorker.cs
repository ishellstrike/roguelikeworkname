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
        private readonly Dictionary<Point, GameLevel> onLoadOrGenerate_;
        private readonly Dictionary<Point, MapSector> onStore_;
        public Dictionary<Point, MapSector> Buffer;
        private bool exit;
        private Point lastTry;

        public LevelWorker() {
            onLoadOrGenerate_ = new Dictionary<Point, GameLevel>();
            onStore_ = new Dictionary<Point, MapSector>();
            Buffer = new Dictionary<Point, MapSector>();
        }

        public bool Loading() {
            return onLoadOrGenerate_.Count > 0;
        }

        public bool Storing() {
            return onStore_.Count > 0;
        }

        public bool Generating() {
            return false;
        }

        public int LoadCount() {
            return onLoadOrGenerate_.Count;
        }

        public int GenerationCount() {
            return 0;
        }

        public int StoreCount() {
            return onStore_.Count;
        }

        public int ReadyCount()
        {
            return Buffer.Count;
        }

        public void Run() {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            Thread.CurrentThread.IsBackground = true;
            while (!exit) {
                if (onLoadOrGenerate_.Count > 0) {
                    KeyValuePair<Point, GameLevel> kvp = onLoadOrGenerate_.ElementAt(0);
                    MapSector trget;
                    if (onStore_.TryGetValue(kvp.Key, out trget)) {
                        Buffer.Add(kvp.Key, trget);
                        onStore_.Remove(kvp.Key);
                    } else {
                        var ms = new MapSector(kvp.Value, kvp.Key.X, kvp.Key.Y);
                        ms.Rebuild(kvp.Value.MapSeed);
                        Buffer.Add(kvp.Key, ms);
                    }
                    lastTry = kvp.Key;
                    onLoadOrGenerate_.Remove(kvp.Key);
                }

                lastTry = new Point(int.MinValue, int.MinValue);

                if (!Generating() && !Loading()) {
                    Thread.Sleep(300);
                    var t = Buffer.Select(x=>x).ToList();
                    foreach (var pair in t) {
                        Buffer.Remove(pair.Key);
                        onStore_.Add(pair.Key, pair.Value);
                    }
                }
            }
        }

        private void LoadOrGenerate(Point p, GameLevel gl) {
            if (!onLoadOrGenerate_.ContainsKey(p)) {
                onLoadOrGenerate_.Add(p, gl);
            }
        }

        public MapSector TryGet(Point p, GameLevel gl) {
            //if (lastTry == p) {
            //    return null;
            //}
            if (Buffer.ContainsKey(p)) {
                return Buffer[p];
            }
            LoadOrGenerate(p, gl);
            //lastTry = p;
            return null;
        }

        public void StoreGenerated(MapSector ms) {
            var point = new Point(ms.SectorOffsetX, ms.SectorOffsetY);
            if(onStore_.ContainsKey(point)) {
                onStore_.Remove(point);
            }
            onStore_.Add(point, ms);
        }

        public void Stop() {
            exit = true;
        }

        public void SectoreSaver(MapSector ms) {
            var uniq_id = new List<string>();
            var uniq_mtex = new List<string>();
            foreach (var block in ms.Blocks) {
                if(!uniq_id.Contains(block.Id)) {
                    uniq_id.Add(block.Id);
                }
                if (!uniq_mtex.Contains(block.MTex)) {
                    uniq_mtex.Add(block.MTex);
                }
            }
            var fniq_id = new List<string>();
            var fniq_mtex = new List<string>();
            foreach (var floor in ms.Floors) {
                if (!fniq_id.Contains(floor.Id)) {
                    fniq_id.Add(floor.Id);
                }
                if (!fniq_mtex.Contains(floor.MTex)) {
                    fniq_mtex.Add(floor.MTex);
                }
            }

            try {
                var fileStream = new StreamWriter(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", ms.SectorOffsetX, ms.SectorOffsetY), false);
                fileStream.Write("~");
                BlockPart(ms, uniq_mtex, uniq_id, fileStream);
                fileStream.WriteLine();
                fileStream.Write("~");
                FloorPart(ms, fniq_mtex, fniq_id, fileStream);
                //gZipStream.Close();
                //gZipStream.Dispose();
                fileStream.Close();
                fileStream.Dispose();
            } catch (Exception e) {
                logger.Error(e.ToString);
            }
        }

        private static void BlockPart(MapSector ms, List<string> uniq_mtex, List<string> uniq_id, StreamWriter fileStream) {
            for (int i = 0; i < ms.Blocks.Count; i++) {
                int id = uniq_id.IndexOf(ms.Blocks[i].Id);
                int count = 1;
                if (i != ms.Blocks.Count - 1) {
                    for (int j = i + 1; j < ms.Blocks.Count; j++) {
                        if (uniq_id.IndexOf(ms.Blocks[j].Id) == id) {
                            count++;
                        }
                        else {
                            break;
                        }
                    }
                }
                if (count > 2) {
                    fileStream.Write(string.Format("!{0}!{1} ", id, count));
                }
                else {
                    for (int k = 0; k < count; k++) {
                        fileStream.Write(id + " ");
                    }
                }
                i += count - 1;
            }
            fileStream.WriteLine();
            fileStream.Write("~");
            foreach (var v in uniq_id) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }

            fileStream.WriteLine();
            fileStream.Write("~");
            for (int i = 0; i < ms.Blocks.Count; i++) {
                int mtex = uniq_mtex.IndexOf(ms.Blocks[i].MTex);
                int count = 1;
                if (i != ms.Blocks.Count - 1) {
                    for (int j = i + 1; j < ms.Blocks.Count; j++) {
                        if (uniq_mtex.IndexOf(ms.Blocks[j].MTex) == mtex) {
                            count++;
                        }
                        else {
                            break;
                        }
                    }
                }
                if (count > 2) {
                    fileStream.Write(string.Format("!{0}!{1} ", mtex, count));
                }
                else {
                    for (int k = 0; k < count; k++) {
                        fileStream.Write(mtex + " ");
                    }
                }
                i += count - 1;
            }
            fileStream.WriteLine();
            fileStream.Write("~");
            foreach (var v in uniq_mtex) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }
        }

        private static void FloorPart(MapSector ms, List<string> fniq_mtex, List<string> fniq_id, StreamWriter fileStream)
        {
            for (int i = 0; i < ms.Floors.Length; i++) {
                int id = fniq_id.IndexOf(ms.Floors[i].Id);
                int count = 1;
                if (i != ms.Floors.Length - 1) {
                    for (int j = i + 1; j < ms.Floors.Length; j++) {
                        if (fniq_id.IndexOf(ms.Floors[j].Id) == id) {
                            count++;
                        } else {
                            break;
                        }
                    }
                }
                if (count > 2) {
                    fileStream.Write(string.Format("!{0}!{1} ", id, count));
                } else {
                    for (int k = 0; k < count; k++) {
                        fileStream.Write(id + " ");
                    }
                }
                i += count - 1;
            }
            fileStream.WriteLine();
            fileStream.Write("~");
            foreach (var v in fniq_id) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }

            fileStream.WriteLine();
            fileStream.Write("~");
            for (int i = 0; i < ms.Floors.Length; i++) {
                int mtex = fniq_mtex.IndexOf(ms.Floors[i].MTex);
                int count = 1;
                if (i != ms.Floors.Length - 1) {
                    for (int j = i + 1; j < ms.Floors.Length; j++) {
                        if (fniq_mtex.IndexOf(ms.Floors[j].MTex) == mtex) {
                            count++;
                        } else {
                            break;
                        }
                    }
                }
                if (count > 2) {
                    fileStream.Write(string.Format("!{0}!{1} ", mtex, count));
                } else {
                    for (int k = 0; k < count; k++) {
                        fileStream.Write(mtex + " ");
                    }
                }
                i += count - 1;
            }
            fileStream.WriteLine();
            fileStream.Write("~");
            foreach (var v in fniq_mtex) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }
        }
    }
}