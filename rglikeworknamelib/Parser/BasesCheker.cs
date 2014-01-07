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
                if (data.Value.ItemScript != null)
                {
                    for (int i = 0; i < data.Value.ItemScript.Length; i++) {
                        var script = data.Value.ItemScript[i];
                        if (!ItemDataBase.Instance.ItemScripts.ContainsKey(script)) {
                            Logger.Error(string.Format("Item script \"{0}\" not found", script));
                            data.Value.ItemScript[i] = "is_nothing";
                            errorIDB++;
                        }
                        
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

            int errorCScript = 0;
            foreach (var crea in CreatureDataBase.Data)
            {
                if (crea.Value.BehaviorScript != null && !CreatureDataBase.Scripts.ContainsKey(crea.Value.BehaviorScript))
                {
                    Logger.Error(string.Format("Behavior script \"{0}\" not found", crea.Value.BehaviorScript));
                    crea.Value.BehaviorScript = "bs_nothing";
                    errorCScript++;
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
            for (int i = 0; i < CraftDataBase.Data.Count; i++) {
                var craftData = CraftDataBase.Data[i];
                if (craftData.Input1 != null) {
                    for (int index = 0; index < craftData.Input1.Count; index++) {
                        var s = craftData.Input1[index];
                        if (!idb.Data.ContainsKey(s)) {
                            var pos = craftData.Input1.IndexOf(s);
                            craftData.Input1.RemoveAt(pos);
                            craftData.Input1Count.RemoveAt(pos);
                            errorCrDB++;
                        }
                    }
                }
                if (craftData.Input2 != null) {
                    for (int index = 0; index < craftData.Input2.Count; index++) {
                        var s = craftData.Input2[index];
                        if (!idb.Data.ContainsKey(s)) {
                            var pos = craftData.Input2.IndexOf(s);
                            craftData.Input2.RemoveAt(pos);
                            craftData.Input2Count.RemoveAt(pos);
                            errorCrDB++;
                        }
                    }
                }

                if (craftData.Input3 != null) {
                    for (int index = 0; index < craftData.Input3.Count; index++) {
                        var s = craftData.Input3[index];
                        if (!idb.Data.ContainsKey(s)) {
                            var pos = craftData.Input3.IndexOf(s);
                            craftData.Input3.RemoveAt(pos);
                            craftData.Input3Count.RemoveAt(pos);
                            errorCrDB++;
                        }
                    }
                }

                if (craftData.Input4 != null) {
                    for (int index = 0; index < craftData.Input4.Count; index++) {
                        var s = craftData.Input4[index];
                        if (!idb.Data.ContainsKey(s)) {
                            var pos = craftData.Input4.IndexOf(s);
                            craftData.Input4.RemoveAt(pos);
                            craftData.Input4Count.RemoveAt(pos);
                            errorCrDB++;
                        }
                    }
                }

                for (int index = 0; index < craftData.Output.Count; index++) {
                    var s = craftData.Output[index];
                    if (!idb.Data.ContainsKey(s)) {
                        var pos = craftData.Output.IndexOf(s);
                        craftData.Output.RemoveAt(pos);
                        craftData.OutputCount.RemoveAt(pos);
                        errorCrDB++;
                    }
                }

                if ((craftData.Input1 != null && craftData.Input1.Count == 0) ||
                    (craftData.Input2 != null && craftData.Input2.Count == 0) ||
                    (craftData.Input3 != null && craftData.Input3.Count == 0) ||
                    (craftData.Input4 != null && craftData.Input4.Count == 0) || craftData.Output.Count == 0) {
                    CraftDataBase.Data.Remove(craftData);
                }
            }

            Logger.Info(string.Format("\nTotal:\n     {4} in SchemesDataBase\n     {0} in BlockDataBase\n     {3} in FloorDataBase\n     {5} in CraftDataBase\n     {1} in ItemDataBase     {6} in Creature behavior scripts\n\nSummary: {2} errors", errorBDB, errorIDB, errorIDB + errorBDB + errorFDB, errorFDB, errorScDB, errorCrDB, errorCScript));
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
