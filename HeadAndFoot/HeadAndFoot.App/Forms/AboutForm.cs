using System;
using System.Drawing;
using System.Windows.Forms;

namespace HeadAndFoot.App.Forms
{
    public partial class AboutForm : Form
    {
        private const int MODAL_WIDTH = 348;
        private const int MODAL_HIGHT = 283;

        public AboutForm()
        {
            InitializeComponent();

            SetupAboutForm();
        }

        private void SetupAboutForm()
        {
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;

            string appName = Properties.Settings.Default.ApplicationFriendlyName.Replace("&", "&&");
            string appVersion = Application.ProductVersion;

            Text = string.Format("About {0}", Properties.Settings.Default.ApplicationFriendlyName);
            AppNameLabel.Text = string.Format("{0}  |  v{1}", appName, appVersion);
            CopyLabel.Text = string.Format("Copyright © {0}", Properties.Settings.Default.AboutCopyrightHolder);
            ByLabel.Text = string.Format("by {0}", Properties.Settings.Default.AboutLeadProgrammerName);
            AppDescriptionLabel.Text = Properties.Settings.Default.AboutDescription;
        }

        private void AppLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.ApplicationLink);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
