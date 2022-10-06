using HeadAndFoot.App.Base;
using HeadAndFoot.App.Forms;
using HeadAndFoot.Core;
using HeadAndFoot.Core.Base.Logging;
using System;
using System.Windows.Forms;

namespace HeadAndFoot.App
{
    public partial class ShellForm : Form
    {
        private Manager _gameManager;
        public Manager GameManager
        {
            get { return _gameManager; }
            set { _gameManager = value; }
        }

        private readonly Padding _defaultContentAreaPadding = new Padding(15);
        private readonly LogUC _logUC;

        public ShellForm()
        {
            InitializeComponent();

            Text = Properties.Settings.Default.ApplicationFriendlyName;
            this.AboutMenuItem.Text = string.Format("&About {0}", Properties.Settings.Default.ApplicationFriendlyName.Replace("&", "&&"));

            _logUC = new LogUC();
            EstablishLogging();

            ModifyMenus();

            GameManager = new Manager();

            MainContentPanel.Controls.Add(GetNewDeal());
        }

        private void EstablishLogging()
        {
            _logUC.Padding = _defaultContentAreaPadding;
            _logUC.Dock = DockStyle.Fill;
            AppLogger.SetTargetInvoking(_logUC.UpdateLogs);
            _logUC.UpdateLogs(AppLogger.GetLogs()); // Load logs that may have been written before delegate could be set.
        }

        private void ModifyMenus()
        {
            //TODO: The following Windows 98 aesthetics.
            // Detached drop downs.
            // 3D / darker tool strip separator.
            // Centered, but still left justified, menu item text and tool strip separator.

            this.MenuStrip.Renderer = new ToolStripProfessionalRenderer(new MenuStripColorTable());

            ((ToolStripDropDownMenu)this.GameMenuItem.DropDown).ShowImageMargin = false;
            ((ToolStripDropDownMenu)this.HelpMenuItem.DropDown).ShowImageMargin = false;

            this.GameMenuItem.DropDownItems.Insert(1, new ToolStripSeparator());
            this.GameMenuItem.DropDownItems.Insert(this.GameMenuItem.DropDownItems.Count - 1, new ToolStripSeparator());
            this.HelpMenuItem.DropDownItems.Insert(this.HelpMenuItem.DropDownItems.Count - 1, new ToolStripSeparator());
        }

        private Control GetNewDeal()
        {
            Control homeContent = new IntroductionUC();
            homeContent.Padding = _defaultContentAreaPadding;
            homeContent.Dock = DockStyle.Fill;

            return homeContent;
        }

        private void DealMenuItem_Click(object sender, EventArgs e)
        {
            MainContentPanel.Controls.Clear();
            MainContentPanel.Controls.Add(GetNewDeal());
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RulesMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Settings.Default.GameRulesLink);
        }

        private void LogMenuItem_Click(object sender, EventArgs e)
        {
            MainContentPanel.Controls.Clear();
            MainContentPanel.Controls.Add(_logUC);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
            {
                aboutForm.ShowDialog(this);
            }
        }
    }
}
