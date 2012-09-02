using System;

namespace InfiniteRPG
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (InfiniteRPG game = new InfiniteRPG())
            {
                game.Run();
            }
        }
    }
#endif
}

