using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;

namespace rglikeworknamelib.Dungeon.Level
{
    public class LevelWorker {
        private List<Tuple<Point,MapSector>> onSave_;
        private List<Tuple<Point, GameLevel>> onLoad_;
        private List<Tuple<Point, GameLevel>> onGeneration_;
        public List<Tuple<Point, MapSector>> Ready; 
        private bool exit;

        public LevelWorker() {
            onLoad_ = new List<Tuple<Point, GameLevel>>();
            onGeneration_ = new List<Tuple<Point, GameLevel>>();
            onSave_ = new List<Tuple<Point, MapSector>>();
            Ready = new List<Tuple<Point, MapSector>>();
        }

        public void Run() {
            while (!exit) {
                if (onSave_.Count > 0) {
                    var t = onSave_.First();
                    SaveSector(t.Item2);
                    onSave_.Remove(t);
                }

                if (onLoad_.Count > 0) {
                    var t = onLoad_.First();
                    Ready.Add(new Tuple<Point, MapSector>(new Point(t.Item1.X, t.Item1.Y), LoadSector(t.Item1.X, t.Item1.Y, t.Item2)));
                    onLoad_.Remove(t);
                }

                if (onGeneration_.Count > 0) {
                    var tt = onGeneration_.First();
                    var ms = new MapSector(tt.Item2, tt.Item1.X, tt.Item1.Y);
                    ms.Rebuild(tt.Item2.MapSeed);
                    Ready.Add(new Tuple<Point, MapSector>(new Point(tt.Item1.X, tt.Item1.Y), ms));
                    onGeneration_.Remove(tt);
                }

                Thread.Sleep(500);
            }
        }

        public void Generate(Point p, GameLevel gl) {
            var tuple = new Tuple<Point, GameLevel>(p, gl);
            if (!onGeneration_.Contains(tuple)) {
                onGeneration_.Add(tuple);
            }
        }

        public void Load(Point p, GameLevel gl) {
            var tuple = new Tuple<Point, GameLevel>(p, gl);
            if(!onLoad_.Contains(tuple)) {
                onLoad_.Add(tuple);
            }
        }

        public void Save(MapSector ms) {
            var p = new Point(ms.SectorOffsetX, ms.SectorOffsetY);
            onSave_.Add(new Tuple<Point, MapSector>(p, ms));
        }

        public MapSector TryGet(Point p, GameLevel gl) {
            Tuple<Point, MapSector> first = null;
            for (int i = 0; i < Ready.Count; i++) {
                Tuple<Point, MapSector> x = Ready[i];
                if (x.Item1 == p) {
                    first = x;
                    break;
                }
            }
            if(first != null) {
                return first.Item2;
            }

            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", p.X, p.Y))) {
                Load(p, gl);
            } else { Generate(p, gl); }

            return null;
        }

        /// <summary>
        /// only for all map save. Remake!
        /// </summary>
        /// <param name="a"></param>
        internal void SaveSector(MapSector a)
        {
            BinaryFormatter binaryFormatter_ = new BinaryFormatter();
            FileStream fileStream_;
            GZipStream gZipStream_;

            fileStream_ =
                new FileStream(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", a.SectorOffsetX, a.SectorOffsetY),
                               FileMode.Create);
            gZipStream_ = new GZipStream(fileStream_, CompressionMode.Compress);
            binaryFormatter_.Serialize(gZipStream_, a.SectorOffsetX);
            binaryFormatter_.Serialize(gZipStream_, a.SectorOffsetY);
            binaryFormatter_.Serialize(gZipStream_, a.Blocks);
            binaryFormatter_.Serialize(gZipStream_, a.Floors);
            binaryFormatter_.Serialize(gZipStream_, a.initialNodes);
            binaryFormatter_.Serialize(gZipStream_, a.biom);
            binaryFormatter_.Serialize(gZipStream_, a.creatures);
            binaryFormatter_.Serialize(gZipStream_, a.decals);
            gZipStream_.Close();
            fileStream_.Close();
        }

        private MapSector LoadSector(int sectorOffsetX, int sectorOffsetY, GameLevel gl)
        {
            if (File.Exists(Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY)))
            {
                BinaryFormatter binaryFormatter_ = new BinaryFormatter();
                FileStream fileStream_;
                GZipStream gZipStream_;

                fileStream_ =
                new FileStream(
                    Settings.GetWorldsDirectory() + string.Format("s{0},{1}.rlm", sectorOffsetX, sectorOffsetY),
                    FileMode.Open);

                gZipStream_ = new GZipStream(fileStream_, CompressionMode.Decompress);
                var q1 = binaryFormatter_.Deserialize(gZipStream_);
                var q2 = binaryFormatter_.Deserialize(gZipStream_);
                var q3 = binaryFormatter_.Deserialize(gZipStream_);
                var q4 = binaryFormatter_.Deserialize(gZipStream_);
                var q5 = binaryFormatter_.Deserialize(gZipStream_);
                var q6 = binaryFormatter_.Deserialize(gZipStream_);
                var q7 = binaryFormatter_.Deserialize(gZipStream_);
                var q8 = binaryFormatter_.Deserialize(gZipStream_);
                gZipStream_.Close();
                fileStream_.Close();
                var t = new MapSector(gl, q1, q2, q3, q4, q5, q6, q7, q8);
                t.ready = true;
                return t;
            }
            return null;
        }

        public void Stop() {
            exit = true;
        }
    }
}
