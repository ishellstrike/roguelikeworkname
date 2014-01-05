using System.Collections.Generic;
using rglikeworknamelib.Dungeon.Items;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dialogs {
    public class DialogData {
    }

    public class DialogDataBase {
        public static Dictionary<string, DialogData> data;

        private static DialogDataBase instance_ = new DialogDataBase();

        /// <summary>
        /// Первое обращение порождает первичную загрузку
        /// </summary>
        public static DialogDataBase Instance
        {
            get { return instance_ ?? (instance_ = new DialogDataBase()); }
        }

        private DialogDataBase() {
            data = new Dictionary<string, DialogData>();

            var temp =
                ParsersCore.UniversalParseDirectory(Settings.GetDialogDataDirectory(),
                                                    UniversalParser.Parser<DialogData>);

            foreach (var pair in temp) {
                data.Add(pair.Key, (DialogData) pair.Value);
            }
        }
    }
}