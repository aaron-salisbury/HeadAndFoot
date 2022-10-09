using AmalgamateLabs.MCGE.Business;
using System;
using System.Windows.Forms;

namespace AmalgamateLabs.MCGE.Presentation
{
    public partial class AboutForm : Form
    {
        private string _applicationLink;

        public AboutForm(GameConfig gameConfig)
        {
            InitializeComponent();

            SetupAboutForm(gameConfig);
        }

        private void SetupAboutForm(GameConfig gameConfig)
        {
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            if (gameConfig != null)
            {
                _applicationLink = gameConfig.ApplicationLink;

                LogoPictureBox.BackgroundImage = gameConfig.AppLogoImage;
                Text = string.Format("About {0}", gameConfig.ApplicationFriendlyName);
                CopyLabel.Text = string.Format("Copyright © {0}", gameConfig.AboutCopyrightHolder);
                ByLabel.Text = string.Format("by {0}", gameConfig.AboutLeadProgrammerName);
                AppDescriptionLabel.Text = gameConfig.AboutDescription;

                string appName = string.Empty;
                if (!string.IsNullOrEmpty(gameConfig.ApplicationFriendlyName))
                {
                    appName = gameConfig.ApplicationFriendlyName.Replace("&", "&&");
                }
                AppNameLabel.Text = string.Format("{0}  |  v{1}", appName, gameConfig.ProductVersion);
            }
        }

        private void AppLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_applicationLink))
            {
                System.Diagnostics.Process.Start(_applicationLink);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
