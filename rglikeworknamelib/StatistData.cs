namespace jarg {
    public class StatistData {
        public float Count;
        public string Description;
        public string Name;
        public StatistType Type;

        public override string ToString() {
            return string.Format("{0} : {1}", Name, Count);
        }
    }
}