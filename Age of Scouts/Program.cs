using System;

namespace Age
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {

#if DEBUG
                using (ImprovedGame game = new ImprovedGame(args))
                {
                    game.Run();
                }
#else
            try
            {
                using (ImprovedGame game = new ImprovedGame(args))
                {

                    game.Run();

                }
            }
            catch (Exception ex)
            {
                string msg =
"Hra spadla kvùli chybì.\r\n\r\nProsím pøepošli tento text vývojáøi na adresu petrhudecek2010@gmail.com.\r\n\r\n" +
                ex.ToString();
                string tempfile = System.IO.Path.GetTempFileName();
                System.IO.File.WriteAllText(tempfile, msg);
                System.Diagnostics.Process.Start("notepad", tempfile);
            }
#endif

        }
    }
#endif

}

