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
            Application.Run(new ShellForm(BuildGameEngineConfig()));
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

        private static AmalgamateLabs.MCGE.Business.GameConfig BuildGameEngineConfig()
        {
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager();

            AmalgamateLabs.MCGE.Business.GameConfig gameConfig = new AmalgamateLabs.MCGE.Business.GameConfig();
            //gameConfig.AppIcon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            gameConfig.AppIcon = global::HeadAndFoot.App.Properties.Resources.icon_256_32;
            gameConfig.AppLogoImage = global::HeadAndFoot.App.Properties.Resources.logo_32;
            gameConfig.ProductVersion = Application.ProductVersion;
            gameConfig.ApplicationFriendlyName = Properties.Settings.Default.ApplicationFriendlyName;
            gameConfig.AboutDescription = Properties.Settings.Default.AboutDescription;
            gameConfig.ApplicationLink = Properties.Settings.Default.ApplicationLink;
            gameConfig.AboutCopyrightHolder = Properties.Settings.Default.AboutCopyrightHolder;
            gameConfig.AboutLeadProgrammerName = Properties.Settings.Default.AboutLeadProgrammerName;
            gameConfig.GameRulesLink = Properties.Settings.Default.GameRulesLink;

            return gameConfig;
        }
    }
}
