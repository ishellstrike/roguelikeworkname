namespace jarg {
    internal static class Program {
        private static void Main(string[] args) {
            var game = new Game1();
            game.Run();
            game.Dispose();
        }
    }
}