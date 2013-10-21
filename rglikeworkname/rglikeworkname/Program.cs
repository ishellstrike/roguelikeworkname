namespace jarg
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var game = new JargMain();
            game.Run();
            game.Dispose();
        }
    }
}