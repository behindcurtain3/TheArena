using System;

namespace TheArena
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TheArenaGame game = new TheArenaGame())
            {
                game.Run();
            }
        }
    }
#endif
}

