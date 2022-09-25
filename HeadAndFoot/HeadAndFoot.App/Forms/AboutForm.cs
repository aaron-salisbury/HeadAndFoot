using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace HeadAndFoot.App.Forms
{
    public partial class AboutForm : Form
    {
        private const int MODAL_WIDTH = 450;
        private const int MODAL_HIGHT = 270;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection _fonts = new PrivateFontCollection();

        private Font _logoFont;

        public AboutForm()
        {
            InitializeComponent();

            BuildLogoFont();

            SetupAboutPage();
        }

        private void BuildLogoFont()
        {
            // stackoverflow.com/questions/556147/how-do-i-embed-my-own-fonts-in-a-winforms-app

            byte[] fontData = Properties.Resources.Rye_Regular;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            _fonts.AddMemoryFont(fontPtr, Properties.Resources.Rye_Regular.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Rye_Regular.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);

            _logoFont = new Font(_fonts.Families[0], 16.0F);
        }

        private void SetupAboutPage()
        {
            Size modalSize = new Size(MODAL_WIDTH, MODAL_HIGHT);
            Size = modalSize;
            MinimumSize = modalSize;
            MaximumSize = modalSize;
            SizeGripStyle = SizeGripStyle.Hide;

            Text = $"About {Properties.Settings.Default.ApplicationFriendlyName}";
            AppNameLabel.Text = Properties.Settings.Default.ApplicationFriendlyName.Replace("&", "&&");
            VersionLabel.Text = $"Version {Application.ProductVersion}";
            AppDescriptionLabel.Text = Properties.Settings.Default.AboutDescription;

            AppNameLabel.Font = _logoFont;
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
