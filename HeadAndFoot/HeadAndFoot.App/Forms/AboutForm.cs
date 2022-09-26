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

            SetupAboutPage();
        }

        private void SetupAboutPage()
        {
            Size modalSize = new Size(MODAL_WIDTH, MODAL_HIGHT);
            Size = modalSize;
            MinimumSize = modalSize;
            MaximumSize = modalSize;
            SizeGripStyle = SizeGripStyle.Hide;

            string appName = Properties.Settings.Default.ApplicationFriendlyName.Replace("&", "&&");
            string appVersion = Application.ProductVersion;

            Text = $"About {Properties.Settings.Default.ApplicationFriendlyName}";
            AppNameLabel.Text = $"{appName} | v. {appVersion}";
            CopyLabel.Text = $"Copyright © {Properties.Settings.Default.AboutCopyrightHolder}";
            ByLabel.Text = $"by {Properties.Settings.Default.AboutLeadProgrammerName}";
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

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }
    }
}
