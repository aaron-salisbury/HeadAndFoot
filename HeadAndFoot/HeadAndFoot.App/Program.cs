using System;
using System.Windows.Forms;

namespace HeadAndFoot.App
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //if (!VerifyEnvironment())
            //{
            //    //TODO: Something more graceful.
            //    throw new Exception("This game is not designed for this operating system.");
            //}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ShellForm());
        }

        private static bool VerifyEnvironment()
        {
            OperatingSystem operatingSystem = Environment.OSVersion;
            if (operatingSystem.Platform != PlatformID.Win32Windows)
            {
                return false;
            }

            return true;
        }
    }
}
