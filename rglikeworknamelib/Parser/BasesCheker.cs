using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;

namespace rglikeworknamelib.Parser
{
    public class BasesCheker
    {
        private static readonly Logger Logger = LogManager.GetLogger("BasesCheker");
        public static void CheckAndResolve() {
            var errorBDB = ErrorBdb();

            int errorIDB = 0;
            foreach (var data in ItemDataBase.Data) {
                if (data.Value.Dress != null && !Atlases.DressAtlas.ContainsKey(data.Value.Dress))
                {
                    Logger.Error(string.Format("texture \"{0}\" for ItemDataBase.Dress not found", data.Value.Dress));
                    data.Value.Dress = "error";
                    errorIDB++;
                }
                if (data.Value.AfteruseId != null)
                {
                    if (!ItemDataBase.Data.ContainsKey(data.Value.AfteruseId)) {
                        Logger.Error(string.Format("object \"{0}\" for ItemDataBase.AfteruseId not found", data.Value.AfteruseId));
                        data.Value.AfteruseId = "0";
                        errorIDB++;
                    }
                }
            }

            int errorFDB = 0;
            foreach (var data in FloorDataBase.Data) {
                if (!Atlases.FloorIndexes.ContainsKey(data.Value.MTex))
                {
                    Logger.Error(string.Format("texture \"{0}\" for FloorDataBase.MTex not found", data.Value.MTex));
                    data.Value.MTex = "error";
                    errorFDB++;
                }
                if (data.Value.AlterMtex != null)
                {
                    for (int i = 0; i < data.Value.AlterMtex.Length; i++)
                    {
                        var s = data.Value.AlterMtex[i];
                        if (!Atlases.FloorIndexes.ContainsKey(s))
                        {
                            Logger.Error(string.Format("texture \"{0}\" for FloorDataBase.AlterMtex not found", s));
                            data.Value.AlterMtex[i] = "error";
                            errorFDB++;
                        }
                    }
                }
            }

            Logger.Info(string.Format("\nTotal:\n     {0} in BlockDataBase\n     {3} in FloorDataBase\n     {1} in ItemDataBase\nSummary: {2} errors", errorBDB, errorIDB, errorIDB + errorBDB + errorFDB, errorFDB));
        }

        public static int ErrorBdb() {
            int errorBDB = 0;
            foreach (var data in BlockDataBase.Data) {
                if (!Atlases.BlockIndexes.ContainsKey(data.Value.MTex)) {
                    Logger.Error(string.Format("texture \"{0}\" for BlockDataBase.MTex not found", data.Value.MTex));
                    data.Value.MTex = "error";
                    errorBDB++;
                }
                if (data.Value.AlterMtex != null) {
                    for (int i = 0; i < data.Value.AlterMtex.Length; i++) {
                        var s = data.Value.AlterMtex[i];
                        if (!Atlases.BlockIndexes.ContainsKey(s)) {
                            Logger.Error(string.Format("texture \"{0}\" for BlockDataBase.AlterMtex not found", s));
                            data.Value.AlterMtex[i] = "error";
                            errorBDB++;
                        }
                    }
                }
            }
            return errorBDB;
        }
    }
}
