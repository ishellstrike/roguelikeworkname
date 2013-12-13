namespace rglikeworknamelib.Generation.Names {
    public class NameData {
        public string Name;
        public NameType NameType;

        public override string ToString() {
            return string.Format("{0} ({1})", Name, NameType);
        }
    }
}