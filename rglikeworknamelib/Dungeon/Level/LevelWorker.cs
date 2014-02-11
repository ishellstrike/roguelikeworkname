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
using rglikeworknamelib.Generation;

namespace rglikeworknamelib.Dungeon.Level {
    public class LevelWorker {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<Point, GameLevel> onLoadOrGenerate_;
        private Dictionary<Point, string> onStore_;
        private Dictionary<Point, MapSector> Buffer;
        public bool ServerGame;
        public JargClient client;
        private bool exit;
        private GameLevel gl;

        /// <summary>
        /// Using local generation - false, Using server generation - true
        /// </summary>
        /// <param name="server"></param>
        public LevelWorker(GameLevel gl_) {
            onLoadOrGenerate_ = new Dictionary<Point, GameLevel>();
            onStore_ = new Dictionary<Point, string>();
            Buffer = new Dictionary<Point, MapSector>();
            gl = gl_;
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

        private void StoreSector(MapSector ms) {
            StringBuilder sb = new StringBuilder();
            SectorSaver(new[] {ms}, sb);
            onStore_.Add(new Point(ms.SectorOffsetX, ms.SectorOffsetY), sb.ToString());
        }

        private bool UnstoreSector(Point key, out MapSector ret) {
            string t;
            onStore_.TryGetValue(key, out t);
            if (t == null) {
                ret = null;
                return false;
            }
            ret = SectorLoader(gl, new[] { t }).ElementAt(0).Value;
            return true;
        }

        private HashSet<Point> megaMap = new HashSet<Point>(); 
        private List<KeyValuePair<Point,MapSector>> Generator(int x, int y) {
            var a = x/15;
            var b = y/15;
            var temp = new List<KeyValuePair<Point, MapSector>>();
            for (int i = -1 + a; i < 1 + a; i++)
            {
                for (int j = -1 + b; j < 1 + b; j++)
                {
                    if (megaMap.Contains(new Point(i, j)))
                    {

                    }
                    else
                    {
                        megaMap.Add(new Point(i, j));
                        var s = (int)(MapGenerators.Noise2D(a, j) * int.MaxValue);
                        var rand = new Random(s);
                        
                        temp.AddRange(MapGenerators2.GenerateCityAt(gl, rand, i * 15, j * 15));
                    }
                }
            }

            return temp;
        }

        private void Run() {
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
            Thread.CurrentThread.IsBackground = true;
            while (!exit) {
                if (onLoadOrGenerate_.Count > 0) {
                    var kvp = onLoadOrGenerate_.ElementAt(0);
                    MapSector trget;
                    if (UnstoreSector(kvp.Key, out trget)) {
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

                    var genret = Generator(kvp.Key.X, kvp.Key.Y);
                    foreach (var pair in genret)
                    {
                        try
                        {
                            StoreSector(pair.Value);
                        }
                        catch (Exception)
                        {
                            onStore_.Remove(pair.Key);
                            StoreSector(pair.Value);
                        }
                    }
                }

                if (!Generating() && !Loading()) {
                    Thread.Sleep(100);
                    lock (Buffer)
                    {
                        var t = Buffer.Select(x=>x).ToList();
                        foreach (var pair in t) {
                            Buffer.Remove(pair.Key);
                            try
                            {
                                StoreSector(pair.Value);
                            }
                            catch (Exception)
                            {
                                onStore_.Remove(pair.Key);
                                StoreSector(pair.Value);
                            }
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

            using (var fs = new FileStream(Settings.GetWorldsDirectory() + "mapdata.rlm", FileMode.Create)) {
                using (var stream = new GZipStream(fs, CompressionMode.Compress)) {
                    using (var sw = new StreamWriter(stream)) {
                        foreach (var str in onStore_) {
                            sw.Write(str.Value);
                        }
                    }
                }
            }

            using (var fs = new FileStream(Settings.GetWorldsDirectory() + "mega.rlm", FileMode.Create)) {
                using (var sw = new StreamWriter(fs)) {
                    foreach (var point in megaMap) {
                        sw.Write(point.X + " " + point.Y+",");
                    }
                }
            }
        }

        public void SectorSaver(IEnumerable<MapSector> array, StringBuilder stringBuilder)
        {
            foreach (var ms in array) {
                var blockIdVocab = new List<string>();
                //var blockMtexVocab = new List<string>();
                foreach (var block in ms.Blocks)
                {
                    if (!blockIdVocab.Contains(block.Id))
                    {
                        blockIdVocab.Add(block.Id);
                    }
                    //if (!blockMtexVocab.Contains(block.MTex))
                    //{
                    //    blockMtexVocab.Add(block.MTex);
                    //}
                }
                var floorIdVocab = new List<string>();
                //var floorMtexVocab = new List<string>();
                foreach (var floor in ms.Floors)
                {
                    if (!floorIdVocab.Contains(floor.Id))
                    {
                        floorIdVocab.Add(floor.Id);
                    }
                    //if (!floorMtexVocab.Contains(floor.MTex))
                    //{
                    //    floorMtexVocab.Add(floor.MTex);
                    //}
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
                    BlockPart(ms, blockIdVocab, stringBuilder);
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    FloorPart(ms, floorIdVocab, stringBuilder);
                    stringBuilder.AppendLine();

                    stringBuilder.Append("~");
                    foreach (var creature in ms.Creatures)
                    {
                        var id = monsterIdVocab.IndexOf(creature.Id);
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
                    var i = 0;
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
        private void BlockPart(MapSector ms, List<string> blockIdVocab, StringBuilder stringBuilder) {
            for (var i = 0; i < ms.Blocks.Length; i++) {
                var id = blockIdVocab.IndexOf(ms.Blocks[i].Id);
                var count = 1;
                if (i != ms.Blocks.Length - 1) {
                    for (var j = i + 1; j < ms.Blocks.Length; j++) {
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
                    for (var k = 0; k < count; k++) {
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
        }
        private void FloorPart(MapSector ms, List<string> floorIdVocab, StringBuilder stringBuilder)
        {
            for (var i = 0; i < ms.Floors.Length; i++) {
                var id = floorIdVocab.IndexOf(ms.Floors[i].Id);
                var count = 1;
                if (i != ms.Floors.Length - 1) {
                    for (var j = i + 1; j < ms.Floors.Length; j++) {
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
                    for (var k = 0; k < count; k++) {
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
        }

        private bool load_started;

        internal void LoadAll(GameLevel gl) {
            if (load_started || ServerGame) {
                return;
            }

            onStore_.Clear();
            string stringBlock, megablock;
            using (var fs = new FileStream(Settings.GetWorldsDirectory() + "mapdata.rlm", FileMode.Open)) {
                using (var stream = new GZipStream(fs, CompressionMode.Decompress)) {
                    using (var sr = new StreamReader(stream)) {
                        load_started = true;
                        stringBlock = sr.ReadToEnd();
                    }
                }
            }
            var temp = onStore_;

            using (var fs = new FileStream(Settings.GetWorldsDirectory() + "mega.rlm", FileMode.Open))
            {
                using (var sr = new StreamReader(fs))
                {
                    load_started = true;
                    megablock = sr.ReadToEnd();
                }
            }
            var v = megablock.Split(',');
            megaMap = new HashSet<Point>();
            foreach (var s in v) {
                if (s == string.Empty) {
                    continue;
                }
                var vv = s.Split(' ');
                
                megaMap.Add(new Point(Convert.ToInt32(vv[0]), Convert.ToInt32(vv[1])));
            }

            Settings.NTS1 = "Loading...";

            var parts = stringBlock.Split('#');

            foreach (var str in parts) {
                    try {
                        var t = str.Substring(0,str.IndexOf(','));
                        var tt = str.Substring(str.IndexOf(',') + 1, str.IndexOf('~') - str.IndexOf(',')-3);
                        var position = new Point(int.Parse(t), int.Parse(tt));
                        temp.Add(position, str);
                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex);
                    }
            }

            Settings.NeedToShowInfoWindow = false;

            gl.MapJustUpdated = true;
            load_started = false;
        }

        public Dictionary<Point, MapSector> SectorLoader(GameLevel gl, IEnumerable<string> parts)
        {
            var temp = new Dictionary<Point, MapSector>();
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
                var FloorIdList = par[3].Split(' ').ToList();
                FloorIdList.Remove(FloorIdList.Last());
                var flIddic = par[4].Split(' ').ToList();
                flIddic.Remove(flIddic.Last());
                var MonsterDataList = par[5].Split(' ').ToList();
                MonsterDataList.Remove(MonsterDataList.Last());
                var monIddic = par[6].Split(' ').ToList();
                monIddic.Remove(monIddic.Last());
                var ItemDataList = par[7].Split(' ').ToList();
                ItemDataList.Remove(ItemDataList.Last());
                var itemIddic = par[8].Split(' ').ToList();
                itemIddic.Remove(itemIddic.Last());

                var position = new Point(int.Parse(pos[0].Trim('#')), int.Parse(pos[1].Trim('\r').Trim('\n')));
                var sector = new MapSector(gl, position.X, position.Y);

                var off = 0;
                foreach (var s in BlockIdList) {
                    if (s != " ") {
                        if (s.StartsWith("!")) {
                            var p2 = s.Split('!');
                            var id = blIddic[int.Parse(p2[1])];
                            var cou = int.Parse(p2[2]);
                            for (var i = 0; i < cou; i++) {
                                sector.Blocks[off] = BlockFactory.GetInstance(id);
                                sector.Blocks[off].MTex = sector.Blocks[off].Data.RandomMtexFromAlters();
                                off++;
                            }
                        }
                        else {
                            var id = blIddic[int.Parse(s)];
                            sector.Blocks[off] = BlockFactory.GetInstance(id);
                            sector.Blocks[off].MTex = sector.Blocks[off].Data.RandomMtexFromAlters();
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
                            for (var i = 0; i < cou; i++) {
                                sector.Floors[off].Id = id;
                                sector.Floors[off].MTex = sector.Floors[off].Data.RandomMtexFromAlters();
                                off++;
                            }
                        }
                        else {
                            var id = flIddic[int.Parse(s)];
                            sector.Floors[off].Id = id;
                            sector.Floors[off].MTex = sector.Floors[off].Data.RandomMtexFromAlters();
                            off++;
                        }
                    }
                }
                foreach (var s in MonsterDataList) {
                    if (s != " ") {
                        var monparts = s.Split(',');
                        var id = monIddic[int.Parse(monparts[0])];
                        var mpos = new Vector3(int.Parse(monparts[1]), int.Parse(monparts[2]), 0);
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

        public void StoreString(Point p, string mapsector) {
            onStore_.Add(p, mapsector);
        }
    }
}