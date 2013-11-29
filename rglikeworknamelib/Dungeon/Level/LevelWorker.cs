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
    }
}