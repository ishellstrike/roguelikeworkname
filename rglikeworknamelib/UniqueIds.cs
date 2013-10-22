namespace rglikeworknamelib {
    public static class UniqueIds {
        private static int iteIds, monIds;

        public static int GetNewItemId() {
            iteIds++;
            return iteIds;
        }

        public static int GetNewMonsterId() {
            monIds++;
            return monIds;
        }
    }
}