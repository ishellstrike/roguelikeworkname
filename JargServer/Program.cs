

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using rglikeworknamelib;
using rglikeworknamelib.Creatures;
using rglikeworknamelib.Dialogs;
using rglikeworknamelib.Dungeon.Buffs;
using rglikeworknamelib.Dungeon.Bullets;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Particles;
using rglikeworknamelib.Generation.Names;
using rglikeworknamelib.Parser;

namespace JargServer
{
    class Server {

        public static int ServerPort = 80;
        static bool running = true;

        public delegate void ControlEventHandler(ConsoleEvent consoleEvent);

        private static UDPServer udps;
        private static GameLevel currentFloor_;
        private static LevelWorker levelWorker_;
        private InventorySystem inventory_;

        public static void Main() {

            Process[] p;
            p = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);

            if (p.Length > 1)
            {
                Console.WriteLine("Jarg server already opened on local machine...");
                Thread.Sleep(3000);
                Environment.Exit(0);
            }

            if (!Directory.Exists(Settings.GetWorldsDirectory()))
            {
                Directory.CreateDirectory(Settings.GetWorldsDirectory());
            }

            ServerHello();

            udps = new UDPServer();
            udps.StartServer(ServerPort);

            DataBasesLoadAndThenInitialGeneration();

            WriteColoredLine("Server ready!", ConsoleColor.Cyan);

            while (running) {
                var command = Console.ReadLine();

                switch (command) {
                    case "stop":
                    case "close":
                    case "exit":
                        ServerGoodbye();
                        levelWorker_.SaveAll();
                        Console.WriteLine("Saving map...");
                        levelWorker_.Stop();
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public static void WriteColored(object s, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.Write(s);
            Console.ResetColor();
        }
        public static void WriteColoredLine(object s, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(s);
            Console.ResetColor();
        }

        public static void ServerHello() {
            Console.Write("Server ");
            WriteColored(rglikeworknamelib.Version.GetLong(), ConsoleColor.Cyan);
            Console.Write(" opened at port ");
            WriteColored(ServerPort, ConsoleColor.Cyan);
            Console.Write("...");
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void ServerGoodbye()
        {
            Console.Write("Server ");
            WriteColored(rglikeworknamelib.Version.GetLong(), ConsoleColor.Cyan);
            Console.WriteLine(" closing");
        }

        public enum ConsoleEvent
        {
            CTRL_C = 0,
            CTRL_BREAK = 1,
            CTRL_CLOSE = 2,
            CTRL_LOGOFF = 5,
            CTRL_SHUTDOWN = 6
        }

        private static void InitialGeneration()
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("Initial map generation", "");
            currentFloor_ = new GameLevel(null, null, null, levelWorker_);
            if (levelWorker_ != null)
            {
                levelWorker_.Stop();
            }
            levelWorker_ = new LevelWorker();
            Action lw_ = levelWorker_.Run;
            lw_.BeginInvoke(null, null);
            currentFloor_.lw_ = levelWorker_;

            currentFloor_.GenerateMegaSectorAround(0, 0);

            sw.Stop();
            Console.WriteLine("Initial generation in {0}", sw.Elapsed);
        }

        private void player__onShoot(object sender, EventArgs e)
        {
            //shootFlashTS = TimeSpan.Zero;
            //lig2.Parameters["slen"].SetValue(GlobalWorldLogic.GetCurrentSlen()/2);
            //lig2.Parameters["shine"].SetValue(1.5f);
            //shootFlash = true;
        }

        private static void DataBasesLoadAndThenInitialGeneration()
        {
            Console.WriteLine("Loading bases...");
            var sw = new Stopwatch();
            sw.Start();
            new CreatureDataBase();
            new FloorDataBase();
            new BlockDataBase();
            new SchemesDataBase();
            new BuffDataBase();
            new NameDataBase();
            new CraftDataBase();
            sw.Stop();
            Console.WriteLine(
                "\nTotal:\n     {1} Monsters\n     {2} Blocks\n     {3} Floors\n     {4} Items\n     {5} Schemes\n     {6} Buffs\n     {7} Dialogs\n     {8} Names\n     {9} Crafts\n     loaded in {0}",
                sw.Elapsed,
                CreatureDataBase.Data.Count,
                BlockDataBase.Data.Count,
                FloorDataBase.Data.Count,
                ItemDataBase.Instance.Data.Count,
                SchemesDataBase.Data.Count,
                BuffDataBase.Data.Count,
                DialogDataBase.data.Count,
                NameDataBase.data.Count,
                CraftDataBase.Data.Count);

            sw.Start();
            BasesCheker.CheckAndResolve();
            sw.Stop();
            Console.WriteLine("Check end in {0}", sw.Elapsed);

            InitialGeneration();
        }

    }
}
