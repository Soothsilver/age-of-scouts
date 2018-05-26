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
"An exception has occured.\n\nPlease forward this information to the application programmer.\n\n" +
                ex.ToString() + Environment.NewLine + Environment.NewLine + ex.StackTrace.ToString();
                string tempfile = System.IO.Path.GetTempFileName();
                System.IO.File.WriteAllText(tempfile, msg);
                System.Diagnostics.Process.Start("notepad", tempfile);
            }
#endif

        }
    }
#endif

}

