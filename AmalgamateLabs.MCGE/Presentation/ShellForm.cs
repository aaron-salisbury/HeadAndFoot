using AmalgamateLabs.MCGE.Base;
using AmalgamateLabs.MCGE.Business;
using System;
using System.Windows.Forms;

namespace AmalgamateLabs.MCGE.Presentation
{
    public partial class ShellForm : Form
    {
        public delegate UserControl NewDealUCMethodCall();

        private NewDealUCMethodCall _newDealUCMethodToCall;
        public NewDealUCMethodCall NewDealUCMethodToCall
        {
            get { return _newDealUCMethodToCall; }
            set
            { 
                _newDealUCMethodToCall = value;
                LoadHomeContentArea();
            }
        }

        private GameConfig _gameConfig;
        public GameConfig GameConfig
        {
            get { return _gameConfig; }
            set { _gameConfig = value; }
        }

        private Cards _cards;
        public Cards Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }

        public ShellForm(GameConfig gameConfig)
        {
            InitializeComponent();

            GameConfig = gameConfig;
            Cards = new Cards();

            Setup();
        }

        private void Setup()
        {
            if (GameConfig != null)
            {
                Text = GameConfig.ApplicationFriendlyName;

                if (!string.IsNullOrEmpty(GameConfig.ApplicationFriendlyName))
                {
                    this.AboutMenuItem.Text = string.Format("&About {0}", GameConfig.ApplicationFriendlyName.Replace("&", "&&"));
                }

                if (GameConfig.AppIcon != null)
                {
                    this.Icon = GameConfig.AppIcon;
                }
            }

            ModifyMenus();
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

        private void LoadHomeContentArea()
        {
            if (NewDealUCMethodToCall != null)
            {
                MainContentPanel.Controls.Clear();
                MainContentPanel.Controls.Add(NewDealUCMethodToCall.Invoke());
            }
        }

        private void DealMenuItem_Click(object sender, EventArgs e)
        {
            LoadHomeContentArea();
        }

        private void DeckMenuItem_Click(object sender, EventArgs e)
        {
            using (DeckSelectionForm deckSelectionForm = new DeckSelectionForm(Cards.Backs))
            {
                deckSelectionForm.ShowDialog(this);
            }
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void RulesMenuItem_Click(object sender, EventArgs e)
        {
            if (GameConfig != null && !string.IsNullOrEmpty(GameConfig.GameRulesLink))
            {
                System.Diagnostics.Process.Start(GameConfig.GameRulesLink);
            }
        }

        private void LogMenuItem_Click(object sender, EventArgs e)
        {
            //MainContentPanel.Controls.Clear();
            //MainContentPanel.Controls.Add(_logUC);
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm(GameConfig))
            {
                aboutForm.ShowDialog(this);
            }
        }
    }
}
