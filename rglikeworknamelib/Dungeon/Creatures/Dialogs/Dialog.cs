using System.Collections.Generic;
using rglikeworknamelib.Parser;

namespace rglikeworknamelib.Dialogs {
    public class DialogData {
    }

    public class DialogDataBase {
        public static Dictionary<string, DialogData> data;

        public DialogDataBase() {
            data = new Dictionary<string, DialogData>();

            List<KeyValuePair<string, object>> temp =
                ParsersCore.UniversalParseDirectory(Settings.GetDialogDataDirectory(),
                                                    UniversalParser.Parser<DialogData>);

            foreach (var pair in temp) {
                data.Add(pair.Key, (DialogData) pair.Value);
            }
        }
    }
}