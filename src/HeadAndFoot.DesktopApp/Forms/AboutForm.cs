using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp.Forms;

internal partial class AboutForm : Form
{
    private readonly Icon _logo;

    public AboutForm()
    {
        InitializeComponent();

        Text = "About " + AppResources.PRODUCT_NAME;
        AppNameLabel.Text = AppResources.PRODUCT_NAME;
        VersionLabel.Text = "Version " + Application.ProductVersion;
        DescriptionLabel.Text = AppResources.DESCRIPTION;
        AttributionsTextBox.Text = AppResources.ReadText(AppResources.ATTRIBUTIONS).Replace("\r\n", "\n").Replace("\n", Environment.NewLine);

        _logo = AppResources.LoadIcon(AppResources.LOGO_ICON);
        LogoPictureBox.Image = _logo.ToBitmap();
    }

    private void AppLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        try
        {
            Process.Start(AppResources.HOMEPAGE);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show(this, AppResources.HOMEPAGE, AppResources.PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
