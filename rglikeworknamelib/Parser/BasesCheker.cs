using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Dungeon.Level;
using rglikeworknamelib.Dungeon.Creatures;

namespace rglikeworknamelib.Parser
{
    public class BasesCheker
    {
        private static readonly Logger Logger = LogManager.GetLogger("BasesCheker");
        public static void CheckAndResolve()
        {
            var errorBDB = ErrorBdb();

            int errorIDB = 0;
            ItemDataBase idb = ItemDataBase.Instance;
            var atlases = Atlases.Instance;
            foreach (var data in idb.Data) {
                if (atlases != null) {
                    if (data.Value.Dress != null && !atlases.DressAtlas.ContainsKey(data.Value.Dress)) {
                        Logger.Error(string.Format("Texture \"{0}\" for ItemDataBase.Dress not found", data.Value.Dress));
                        data.Value.Dress = "error";
                        errorIDB++;
                    }
                }
                if (data.Value.AfteruseId != null) {
                    if (!idb.Data.ContainsKey(data.Value.AfteruseId)) {
                        Logger.Error(string.Format("Object \"{0}\" for ItemDataBase.AfteruseId not found", data.Value.AfteruseId));
                        data.Value.AfteruseId = null;
                        errorIDB++;
                    }
                }
            }

            int errorFDB = 0;
            foreach (var data in FloorDataBase.Data) {
                if (atlases == null) {
                    continue;
                }
                if (!atlases.MajorIndexes.ContainsKey(data.Value.MTex)) {
                    Logger.Error(string.Format("Texture \"{0}\" for FloorDataBase.MTex not found", data.Value.MTex));
                    data.Value.MTex = "error";
                    errorFDB++;
                }
                if (data.Value.AlterMtex != null) {
                    for (int i = 0; i < data.Value.AlterMtex.Length; i++) {
                        var s = data.Value.AlterMtex[i];
                        if (!atlases.MajorIndexes.ContainsKey(s)) {
                            Logger.Error(string.Format("Texture \"{0}\" for FloorDataBase.AlterMtex not found", s));
                            data.Value.AlterMtex[i] = "error";
                            errorFDB++;
                        }
                    }
                }
            }

            int errorScDB = 0;
            foreach (var schemese in SchemesDataBase.Data) {
                for (int i = 0; i < schemese.data.Length; i++) {
                    if (!BlockDataBase.Data.ContainsKey(schemese.data[i])) {
                        Logger.Error(string.Format("Block \"{0}\" for SchemesDataBase not found", schemese.data[i]));
                        schemese.data[i] = "error";
                        errorScDB++;
                    }
                }
            }

            int errorCrDB = 0;
            ItemDataBase itemDataBase = ItemDataBase.Instance;
            for (int i = 0; i < itemDataBase.Craft.Count; i++) {
                var craftData = itemDataBase.Craft[i];
                foreach (var s in craftData.Input)
                {
                    for (int j = 0; j < s.Alters.Count; j++)
                    {
                        var alt = s.Alters[j];
                        if (!idb.Data.ContainsKey(alt.Id))
                        {
                            s.Alters.Remove(alt);
                            errorCrDB++;
                        }
                    }
                }

                foreach (var s in craftData.Output)
                {
                    for (int j = 0; j < s.Alters.Count; j++)
                    {
                        var alt = s.Alters[j];
                        if (!idb.Data.ContainsKey(alt.Id))
                        {
                            s.Alters.Remove(alt);
                            errorCrDB++;
                        }
                    }
                }

                if (craftData.Input.Any(x => x.Alters.Count == 0) || craftData.Output.Any(x => x.Alters.Count == 0))
                {
                    itemDataBase.Craft.Remove(craftData);
                }
            }

            Logger.Info(string.Format("\nTotal:\n     {4} in SchemesDataBase\n     {0} in BlockDataBase\n     {3} in FloorDataBase\n     {5} in ItemCraftDataBase\n     {1} in ItemDataBase\n\nSummary: {2} errors", errorBDB, errorIDB, errorIDB + errorBDB + errorFDB, errorFDB, errorScDB, errorCrDB));
        }

        public static int ErrorBdb()
        {
            int errorBDB = 0;
            var instance = Atlases.Instance;
            foreach (var data in BlockDataBase.Data) {
                if (instance == null) {
                    continue;
                }
                if (!instance.MajorIndexes.ContainsKey(data.Value.MTex)) {
                    Logger.Error(string.Format("texture \"{0}\" for BlockDataBase.MTex not found", data.Value.MTex));
                    data.Value.MTex = "error";
                    errorBDB++;
                }
                if (data.Value.AlterMtex != null) {
                    for (int i = 0; i < data.Value.AlterMtex.Length; i++) {
                        var s = data.Value.AlterMtex[i];
                        if (!instance.MajorIndexes.ContainsKey(s)) {
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
