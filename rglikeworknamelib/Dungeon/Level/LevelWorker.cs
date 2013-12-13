using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Dungeon.Level {
    public class LevelWorker {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<Point, GameLevel> onLoadOrGenerate_;
        private Dictionary<Point, MapSector> onStore_;
        public Dictionary<Point, MapSector> Buffer;
        private bool exit;

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
                        if (load_started) {
                            onLoadOrGenerate_.Remove(kvp.Key);
                            continue;
                        }
                        var ms = new MapSector(kvp.Value, kvp.Key.X, kvp.Key.Y);
                        ms.Rebuild(kvp.Value.MapSeed);
                        Buffer.Add(kvp.Key, ms);
                    }
                    onLoadOrGenerate_.Remove(kvp.Key);
                }

                if (!Generating() && !Loading()) {
                    Thread.Sleep(300);
                    var t = Buffer.Select(x=>x).ToList();
                    foreach (var pair in t) {
                        Buffer.Remove(pair.Key);
                        onStore_.Add(pair.Key, pair.Value);
                    }
                }
            }
            stopped = true;
        }

        private bool stopped;
        private void LoadOrGenerate(Point p, GameLevel gl) {
            if (!onLoadOrGenerate_.ContainsKey(p)) {
                onLoadOrGenerate_.Add(p, gl);
            }
        }

        public MapSector TryGet(Point p, GameLevel gl) {
            if (Buffer.ContainsKey(p)) {
                return Buffer[p];
            }
            LoadOrGenerate(p, gl);
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

        private bool save_started;
        public void SaveAll()
        {
            if (save_started)
            {
                return;
            }

            Stop();
            while (!stopped)
            {
                Thread.Sleep(50);
            }
            int i = 0;
            //
            FileStream fs = new FileStream(Settings.GetWorldsDirectory() + "mapdata.rlm", FileMode.Create);
            GZipStream stream = new GZipStream(fs, CompressionMode.Compress);
            StreamWriter sw = new StreamWriter(stream);
            save_started = true;
            foreach (var mapSector in onStore_)
            {
                SectorSaver(mapSector.Value, sw);
                Settings.NTS2 = i + "/" + onStore_.Count;
                i++;
            }
            sw.Close();
            stream.Close();
            fs.Close();
            sw.Dispose();
            stream.Dispose();
            fs.Dispose();
        }
        private void SectorSaver(MapSector ms, StreamWriter fileStream)
        {
            var blockIdVocab = new List<string>();
            var blockMtexVocab = new List<string>();
            foreach (var block in ms.Blocks) {
                if(!blockIdVocab.Contains(block.Id)) {
                    blockIdVocab.Add(block.Id);
                }
                if (!blockMtexVocab.Contains(block.MTex)) {
                    blockMtexVocab.Add(block.MTex);
                }
            }
            var floorIdVocab = new List<string>();
            var floorMtexVocab = new List<string>();
            foreach (var floor in ms.Floors) {
                if (!floorIdVocab.Contains(floor.Id)) {
                    floorIdVocab.Add(floor.Id);
                }
                if (!floorMtexVocab.Contains(floor.MTex)) {
                    floorMtexVocab.Add(floor.MTex);
                }
            }
            var monsterIdVocab = new List<string>();
            foreach (var creature in ms.Creatures) {
                if (!monsterIdVocab.Contains(creature.Id)) {
                    monsterIdVocab.Add(creature.Id);
                } 
            }

            var itemIdVocab = new List<string>();
            foreach (var block in ms.Blocks) {
                if (block is StorageBlock) {
                    foreach (var item in (block as StorageBlock).StoredItems) {
                        if (!itemIdVocab.Contains(item.Id)) {
                            itemIdVocab.Add(item.Id);
                        }
                    }
                }
            }

            try {
                fileStream.Write("#");
                fileStream.Write(ms.SectorOffsetX + "," + ms.SectorOffsetY);
                fileStream.WriteLine();

                fileStream.Write("~");
                BlockPart(ms, blockMtexVocab, blockIdVocab, fileStream);
                fileStream.WriteLine();

                fileStream.Write("~");
                FloorPart(ms, floorMtexVocab, floorIdVocab, fileStream);
                fileStream.WriteLine();

                fileStream.Write("~");
                foreach (var creature in ms.Creatures) {
                    int id = monsterIdVocab.IndexOf(creature.Id);
                    fileStream.Write(id);
                    fileStream.Write(",");
                    fileStream.Write((int) creature.Position.X);
                    fileStream.Write(",");
                    fileStream.Write((int) creature.Position.Y);
                    fileStream.Write(" ");
                }
                fileStream.WriteLine();

                fileStream.Write("~");
                foreach (var v in monsterIdVocab) {
                    fileStream.Write(v);
                    fileStream.Write(" ");
                }
                fileStream.WriteLine();

                fileStream.Write("~");
                int i = 0;
                foreach (var block in ms.Blocks) {
                    if (block is StorageBlock) {
                        foreach (var item in (block as StorageBlock).StoredItems) {
                            fileStream.Write(itemIdVocab.IndexOf(item.Id));
                            fileStream.Write(",");
                            fileStream.Write(i);
                            fileStream.Write(",");
                            fileStream.Write(item.Count);
                            fileStream.Write(",");
                            fileStream.Write(item.Doses);
                            fileStream.Write(" ");
                        }
                    }
                    i++;
                }
                fileStream.WriteLine();

                fileStream.Write("~");
                foreach (var v in itemIdVocab)
                {
                    fileStream.Write(v);
                    fileStream.Write(" ");
                }
                fileStream.WriteLine();
            }
            catch (Exception e) {
                logger.Error(e.ToString);
            }
        }
        private static void BlockPart(MapSector ms, List<string> blockMtexVocab, List<string> blockIdVocab, StreamWriter fileStream) {
            for (int i = 0; i < ms.Blocks.Count; i++) {
                int id = blockIdVocab.IndexOf(ms.Blocks[i].Id);
                int count = 1;
                if (i != ms.Blocks.Count - 1) {
                    for (int j = i + 1; j < ms.Blocks.Count; j++) {
                        if (blockIdVocab.IndexOf(ms.Blocks[j].Id) == id) {
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
            foreach (var v in blockIdVocab) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }

            fileStream.WriteLine();
            fileStream.Write("~");
            for (int i = 0; i < ms.Blocks.Count; i++) {
                int mtex = blockMtexVocab.IndexOf(ms.Blocks[i].MTex);
                int count = 1;
                if (i != ms.Blocks.Count - 1) {
                    for (int j = i + 1; j < ms.Blocks.Count; j++) {
                        if (blockMtexVocab.IndexOf(ms.Blocks[j].MTex) == mtex) {
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
            foreach (var v in blockMtexVocab) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }
        }
        private static void FloorPart(MapSector ms, List<string> floorMtexVocab, List<string> floorIdVocab, StreamWriter fileStream)
        {
            for (int i = 0; i < ms.Floors.Length; i++) {
                int id = floorIdVocab.IndexOf(ms.Floors[i].Id);
                int count = 1;
                if (i != ms.Floors.Length - 1) {
                    for (int j = i + 1; j < ms.Floors.Length; j++) {
                        if (floorIdVocab.IndexOf(ms.Floors[j].Id) == id) {
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
            foreach (var v in floorIdVocab) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }

            fileStream.WriteLine();
            fileStream.Write("~");
            for (int i = 0; i < ms.Floors.Length; i++) {
                int mtex = floorMtexVocab.IndexOf(ms.Floors[i].MTex);
                int count = 1;
                if (i != ms.Floors.Length - 1) {
                    for (int j = i + 1; j < ms.Floors.Length; j++) {
                        if (floorMtexVocab.IndexOf(ms.Floors[j].MTex) == mtex) {
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
            foreach (var v in floorMtexVocab) {
                fileStream.Write(v);
                fileStream.Write(" ");
            }
        }

        private bool load_started;

        internal void LoadAll(GameLevel gl) {
            if (load_started) {
                return;
            }

            onStore_.Clear();
            FileStream fs = new FileStream(Settings.GetWorldsDirectory() + "mapdata.rlm", FileMode.Open);
            GZipStream stream = new GZipStream(fs, CompressionMode.Decompress);
            StreamReader sr = new StreamReader(stream);
            load_started = true;
            string block = sr.ReadToEnd();
            var temp = onStore_;

            var parts = block.Split('#');

            foreach (var part in parts) {
                if (part.Length < 2) {continue;}
                var par = part.Split('~');

                var pos = par[0].Split(',');
                var BlockIdList = par[1].Split(' ').ToList();
                BlockIdList.Remove(BlockIdList.Last());
                var blIddic = par[2].Split(' ').ToList();
                blIddic.Remove(blIddic.Last());
                var BlockTexList = par[3].Split(' ').ToList();
                BlockTexList.Remove(BlockTexList.Last());
                var blTexdic = par[4].Split(' ').ToList();
                blTexdic.Remove(blTexdic.Last());
                var FloorIdList = par[5].Split(' ').ToList();
                FloorIdList.Remove(FloorIdList.Last());
                var flIddic = par[6].Split(' ').ToList();
                flIddic.Remove(flIddic.Last());
                var FloorTexList = par[7].Split(' ').ToList();
                FloorTexList.Remove(FloorTexList.Last());
                var flTexdic = par[8].Split(' ').ToList();
                flTexdic.Remove(flTexdic.Last());
                var MonsterDataList = par[9].Split(' ').ToList();
                MonsterDataList.Remove(MonsterDataList.Last());
                var monIddic = par[10].Split(' ').ToList();
                monIddic.Remove(monIddic.Last());
                var ItemDataList = par[11].Split(' ').ToList();
                ItemDataList.Remove(ItemDataList.Last());
                var itemIddic = par[12].Split(' ').ToList();
                itemIddic.Remove(itemIddic.Last());

                Point position = new Point(int.Parse(pos[0]), int.Parse(pos[1]));
                MapSector sector = new MapSector(gl, position.X, position.Y);

                Settings.NTS1 = "Loading : ";
                Settings.NeedToShowInfoWindow = true;

                int off = 0;
                foreach (var s in BlockIdList)
                {
                    if (s != " ") {
                        if (s.StartsWith("!")) {
                            var p2 = s.Split('!');
                            var id = blIddic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++) {
                                sector.Blocks[off] = BlockFactory.GetInstance(id);
                                off++;
                            }
                        } else {
                            var id = blIddic[int.Parse(s)];
                            sector.Blocks[off] = BlockFactory.GetInstance(id);
                            off++;
                        }
                    }
                }
                off = 0;
                foreach (var s in BlockTexList)
                {
                    if (s != " ")
                    {
                        if (s.StartsWith("!"))
                        {
                            var p2 = s.Split('!');
                            var mTex = blTexdic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++)
                            {
                                sector.Blocks[off].MTex = mTex;
                                off++;
                            }
                        }
                        else
                        {
                            var mTex = blTexdic[int.Parse(s)];
                            sector.Blocks[off].MTex = mTex;
                            off++;
                        }
                    }
                }
                off = 0;
                foreach (var s in FloorIdList)
                {
                    if (s != " ")
                    {
                        if (s.StartsWith("!"))
                        {
                            var p2 = s.Split('!');
                            var id = flIddic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++)
                            {
                                sector.Floors[off].Id = id;
                                off++;
                            }
                        }
                        else
                        {
                            var id = flIddic[int.Parse(s)];
                            sector.Floors[off].Id = id;
                            off++;
                        }
                    }
                }
                off = 0;
                foreach (var s in FloorTexList)
                {
                    if (s != " ")
                    {
                        if (s.StartsWith("!"))
                        {
                            var p2 = s.Split('!');
                            var mTex = flTexdic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++)
                            {
                                sector.Floors[off].MTex = mTex;
                                off++;
                            }
                        }
                        else
                        {
                            var mTex = flTexdic[int.Parse(s)];
                            sector.Floors[off].MTex = mTex;
                            off++;
                        }
                    }
                }
                foreach (var s in MonsterDataList)
                {
                    if (s != " ") {
                        var monparts = s.Split(',');
                        var id = monIddic[int.Parse(monparts[0])];
                        var mpos = new Vector2(int.Parse(monparts[1]), int.Parse(monparts[2]));
                        var mon = CreatureFactory.GetInstance(id, mpos);
                        sector.Creatures.Add(mon);
                    }
                }
                foreach (var s in ItemDataList)
                {
                    if (s != " ")
                    {
                        var itemparts = s.Split(',');
                        var id = itemIddic[int.Parse(itemparts[0])];
                        var onedim = int.Parse(itemparts[1]);
                        var icount = int.Parse(itemparts[2]);
                        var idoses = int.Parse(itemparts[3]);
                        var item = ItemFactory.GetInstance(id, icount);
                        item.Doses = idoses;
                        ((StorageBlock)(sector.Blocks[onedim])).StoredItems.Add(item);
                    }
                }
                temp.Add(position, sector);
                Settings.NTS2 = temp.Count.ToString();
            }
            Settings.NeedToShowInfoWindow = false;

            sr.Close();
            stream.Close();
            fs.Close();
            sr.Dispose();
            stream.Dispose();
            fs.Dispose();
            gl.MapJustUpdated = true;
            load_started = false;
        }
    }
}