using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using NLog;
using rglikeworknamelib.Dungeon.Creatures;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level.Blocks;

namespace rglikeworknamelib.Dungeon.Level {
    public class LevelWorker {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<Point, GameLevel> onLoadOrGenerate_;
        private Dictionary<Point, MapSector> onStore_;
        public Dictionary<Point, MapSector> Buffer;
        public bool ServerGame;
        public JargClient client;
        private bool exit;

        /// <summary>
        /// Using local generation - false, Using server generation - true
        /// </summary>
        /// <param name="server"></param>
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

        public int LoadCount {
            get {return onLoadOrGenerate_.Count;}
        }

        public int GenerationCount() {
            return 0;
        }

        public int StoreCount {
            get { return onStore_.Count; }
        }

        public int ReadyCount
        {
            get {return Buffer.Count;}
        }

        public void Start()
        {
            new Thread(Run).Start();
        }

        private void Run() {
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
                        if (load_started || ServerGame) {
                            onLoadOrGenerate_.Remove(kvp.Key);
                             if (client != null) {
                                 client.SendStruct(new JargPack { action = "mapsector", name = client.name, x = kvp.Key.X, y = kvp.Key.Y });
                             }
                            continue;
                        }
                        var ms = new MapSector(kvp.Value, kvp.Key.X, kvp.Key.Y);
                        generated++;
                        ms.Rebuild(kvp.Value.MapSeed);
                        Buffer.Add(kvp.Key, ms);
                    }
                    onLoadOrGenerate_.Remove(kvp.Key);
                }

                if (!Generating() && !Loading()) {
                    Thread.Sleep(300);
                    lock (Buffer)
                    {
                        var t = Buffer.Select(x=>x).ToList();
                        foreach (var pair in t) {
                            Buffer.Remove(pair.Key);
                            onStore_.Add(pair.Key, pair.Value);
                        }
                    }
                }
            }
            stopped = true;
        }

        private bool stopped;

        public MapSector TryGet(Point p, GameLevel gl) {
            lock (Buffer)
            {
                if (Buffer.ContainsKey(p))
                {
                    return Buffer[p];
                }
            }
            if (!onLoadOrGenerate_.ContainsKey(p)) {
                onLoadOrGenerate_.Add(p, gl);
            }
            return null;
        }

        public void StoreGenerated(MapSector ms) {
            var point = new Point(ms.SectorOffsetX, ms.SectorOffsetY);
            if(Buffer.ContainsKey(point)) {
                Buffer.Remove(point);
            }
            Buffer.Add(point, ms);
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

            Settings.NTS1 = "Saving...";

            Stop();
            while (!stopped)
            {
                Thread.Sleep(50);
            }
            //int i = 0;
            //
            FileStream fs = new FileStream(Settings.GetWorldsDirectory() + "mapdata.rlm", FileMode.Create);
            GZipStream stream = new GZipStream(fs, CompressionMode.Compress);
            StreamWriter sw = new StreamWriter(stream);

            var procCount = Environment.ProcessorCount;
            var chunkLength = (int)Math.Ceiling(onStore_.Count() / (double)procCount);
            var chunks = Enumerable.Range(0, procCount).Select(x => onStore_.Values.Skip(x * chunkLength).Take(chunkLength));
            Action<IEnumerable<MapSector>, StringBuilder> pl = SectorSaver;
            var sbs = new List<StringBuilder>();
            for (int j = 0; j < procCount; j++) {
                sbs .Add(new StringBuilder());
            }

            save_started = true;
            int k = 0;
            List<IAsyncResult> results = chunks.Select(chunk =>
            {
                k++;
                return pl.BeginInvoke(chunk, sbs[k - 1], null, null);
            }).ToList();


            for (;;) {
                if (results.All(x => x.IsCompleted)) {
                    break;
                }
                Thread.Sleep(50);
            }

            foreach (var stringBuilder in sbs) {
                sw.Write(stringBuilder.ToString());
            }

            sw.Close();
            stream.Close();
            fs.Close();
            sw.Dispose();
            stream.Dispose();
            fs.Dispose();
        }

        public void SectorSaver(IEnumerable<MapSector> array, StringBuilder stringBuilder)
        {
            foreach (var ms in array) {
                var blockIdVocab = new List<string>();
                var blockMtexVocab = new List<string>();
                foreach (var block in ms.Blocks)
                {
                    if (!blockIdVocab.Contains(block.Id))
                    {
                        blockIdVocab.Add(block.Id);
                    }
                    if (!blockMtexVocab.Contains(block.MTex))
                    {
                        blockMtexVocab.Add(block.MTex);
                    }
                }
                var floorIdVocab = new List<string>();
                var floorMtexVocab = new List<string>();
                foreach (var floor in ms.Floors)
                {
                    if (!floorIdVocab.Contains(floor.Id))
                    {
                        floorIdVocab.Add(floor.Id);
                    }
                    if (!floorMtexVocab.Contains(floor.MTex))
                    {
                        floorMtexVocab.Add(floor.MTex);
                    }
                }
                var monsterIdVocab = new List<string>();
                foreach (var creature in ms.Creatures)
                {
                    if (!monsterIdVocab.Contains(creature.Id))
                    {
                        monsterIdVocab.Add(creature.Id);
                    }
                }

                var itemIdVocab = new List<string>();
                foreach (var block in ms.Blocks)
                {
                    if (block is Block)
                    {
                        foreach (var item in (block as Block).StoredItems)
                        {
                            if (!itemIdVocab.Contains(item.Id))
                            {
                                itemIdVocab.Add(item.Id);
                            }
                        }
                    }
                }

                try
                {
                    stringBuilder.Append("#");
                    stringBuilder.Append(ms.SectorOffsetX + "," + ms.SectorOffsetY);
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    BlockPart(ms, blockMtexVocab, blockIdVocab, stringBuilder);
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    FloorPart(ms, floorMtexVocab, floorIdVocab, stringBuilder);
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    foreach (var creature in ms.Creatures)
                    {
                        int id = monsterIdVocab.IndexOf(creature.Id);
                        stringBuilder.Append(id);
                        stringBuilder.Append(",");
                        stringBuilder.Append((int)creature.Position.X);
                        stringBuilder.Append(",");
                        stringBuilder.Append((int)creature.Position.Y);
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    foreach (var v in monsterIdVocab)
                    {
                        stringBuilder.Append(v);
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    int i = 0;
                    foreach (var block in ms.Blocks)
                    {
                            foreach (var item in block.StoredItems)
                            {
                                stringBuilder.Append(itemIdVocab.IndexOf(item.Id));
                                stringBuilder.Append(",");
                                stringBuilder.Append(i);
                                stringBuilder.Append(",");
                                stringBuilder.Append(item.Count);
                                stringBuilder.Append(",");
                                stringBuilder.Append(item.Doses);
                                stringBuilder.Append(" ");
                            }
                        i++;
                    }
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    foreach (var v in itemIdVocab)
                    {
                        stringBuilder.Append(v);
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.AppendLine();
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString);
                }
            }
        }
        private void BlockPart(MapSector ms, List<string> blockMtexVocab, List<string> blockIdVocab, StringBuilder stringBuilder) {
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
                    stringBuilder.Append(string.Format("!{0}!{1} ", id, count));
                }
                else {
                    for (int k = 0; k < count; k++) {
                        stringBuilder.Append(id + " ");
                    }
                }
                i += count - 1;
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("~");
            foreach (var v in blockIdVocab) {
                stringBuilder.Append(v);
                stringBuilder.Append(" ");
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("~");
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
                    stringBuilder.Append(string.Format("!{0}!{1} ", mtex, count));
                }
                else {
                    for (int k = 0; k < count; k++) {
                        stringBuilder.Append(mtex + " ");
                    }
                }
                i += count - 1;
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("~");
            foreach (var v in blockMtexVocab) {
                stringBuilder.Append(v);
                stringBuilder.Append(" ");
            }
        }
        private void FloorPart(MapSector ms, List<string> floorMtexVocab, List<string> floorIdVocab, StringBuilder stringBuilder)
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
                    stringBuilder.Append(string.Format("!{0}!{1} ", id, count));
                } else {
                    for (int k = 0; k < count; k++) {
                        stringBuilder.Append(id + " ");
                    }
                }
                i += count - 1;
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("~");
            foreach (var v in floorIdVocab) {
                stringBuilder.Append(v);
                stringBuilder.Append(" ");
            }

            stringBuilder.AppendLine();
            stringBuilder.Append("~");
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
                    stringBuilder.Append(string.Format("!{0}!{1} ", mtex, count));
                } else {
                    for (int k = 0; k < count; k++) {
                        stringBuilder.Append(mtex + " ");
                    }
                }
                i += count - 1;
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("~");
            foreach (var v in floorMtexVocab) {
                stringBuilder.Append(v);
                stringBuilder.Append(" ");
            }
        }

        private bool load_started;

        internal void LoadAll(GameLevel gl) {
            if (load_started || ServerGame) {
                return;
            }

            onStore_.Clear();
            var fs = new FileStream(Settings.GetWorldsDirectory() + "mapdata.rlm", FileMode.Open);
            var stream = new GZipStream(fs, CompressionMode.Decompress);
            var sr = new StreamReader(stream);
            load_started = true;
            string block = sr.ReadToEnd();
            var temp = onStore_;

            Settings.NTS1 = "Loading...";

            var parts = block.Split('#');
            var procCount = Environment.ProcessorCount;
            var chunkLength = (int)Math.Ceiling(parts.Count() / (double)procCount);
            var chunks = Enumerable.Range(0, procCount).Select(i => parts.Skip(i * chunkLength).Take(chunkLength));
            Func<GameLevel, IEnumerable<string>, Dictionary<Point, MapSector>> pl = PartLoader;
            
            Settings.NeedToShowInfoWindow = true;


            List<IAsyncResult> results = chunks.Select(chunk => pl.BeginInvoke(gl, chunk, null, null)).ToList();

            for (;;) {
                if (results.All(x => x.IsCompleted)) {
                    break;
                }
                Thread.Sleep(50);
            }

            foreach (var asyncResult in results) {
                var result = pl.EndInvoke(asyncResult);
                foreach (var mapSector in result) {
                    try
                    {
                        temp.Add(mapSector.Key, mapSector.Value);
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
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

        public Dictionary<Point, MapSector> PartLoader(GameLevel gl, IEnumerable<string> parts)
        {
            Dictionary<Point, MapSector> temp = new Dictionary<Point, MapSector>();
            foreach (var part in parts) {
                if (part.Length < 2) {
                    continue;
                }
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

                Point position = new Point(int.Parse(pos[0].Trim('#')), int.Parse(pos[1].Trim('\r').Trim('\n')));
                MapSector sector = new MapSector(gl, position.X, position.Y);

                int off = 0;
                foreach (var s in BlockIdList) {
                    if (s != " ") {
                        if (s.StartsWith("!")) {
                            var p2 = s.Split('!');
                            var id = blIddic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++) {
                                sector.Blocks[off] = BlockFactory.GetInstance(id);
                                off++;
                            }
                        }
                        else {
                            var id = blIddic[int.Parse(s)];
                            sector.Blocks[off] = BlockFactory.GetInstance(id);
                            off++;
                        }
                    }
                }
                off = 0;
                foreach (var s in BlockTexList) {
                    if (s != " ") {
                        if (s.StartsWith("!")) {
                            var p2 = s.Split('!');
                            var mTex = blTexdic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++) {
                                sector.Blocks[off].MTex = mTex;
                                off++;
                            }
                        }
                        else {
                            var mTex = blTexdic[int.Parse(s)];
                            sector.Blocks[off].MTex = mTex;
                            off++;
                        }
                    }
                }
                off = 0;
                foreach (var s in FloorIdList) {
                    if (s != " ") {
                        if (s.StartsWith("!")) {
                            var p2 = s.Split('!');
                            var id = flIddic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++) {
                                sector.Floors[off].Id = id;
                                off++;
                            }
                        }
                        else {
                            var id = flIddic[int.Parse(s)];
                            sector.Floors[off].Id = id;
                            off++;
                        }
                    }
                }
                off = 0;
                foreach (var s in FloorTexList) {
                    if (s != " ") {
                        if (s.StartsWith("!")) {
                            var p2 = s.Split('!');
                            var mTex = flTexdic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (int i = 0; i < cou; i++) {
                                sector.Floors[off].MTex = mTex;
                                off++;
                            }
                        }
                        else {
                            var mTex = flTexdic[int.Parse(s)];
                            sector.Floors[off].MTex = mTex;
                            off++;
                        }
                    }
                }
                foreach (var s in MonsterDataList) {
                    if (s != " ") {
                        var monparts = s.Split(',');
                        var id = monIddic[int.Parse(monparts[0])];
                        var mpos = new Vector2(int.Parse(monparts[1]), int.Parse(monparts[2]));
                        var mon = CreatureFactory.GetInstance(id, mpos);
                        sector.Creatures.Add(mon);
                    }
                }
                foreach (var s in ItemDataList) {
                    if (s != " ") {
                        var itemparts = s.Split(',');
                        var id = itemIddic[int.Parse(itemparts[0])];
                        var onedim = int.Parse(itemparts[1]);
                        var icount = int.Parse(itemparts[2]);
                        var idoses = int.Parse(itemparts[3]);
                        var item = ItemFactory.GetInstance(id, icount);
                        item.Doses = idoses;
                        sector.Blocks[onedim].StoredItems.Add(item);
                    }
                }
                temp.Add(position, sector);
            }
            return temp;
        }

        private int generated;
        public int Generated { get { return generated; } }
    }
}