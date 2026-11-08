namespace HeadAndFoot.DesktopApp;

partial class ShellForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }

            DisposeGame();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.MainMenu = new System.Windows.Forms.MainMenu(this.components);
        this.GameMenuItem = new System.Windows.Forms.MenuItem();
        this.DealMenuItem = new System.Windows.Forms.MenuItem();
        this.GameSeparatorMenuItem = new System.Windows.Forms.MenuItem();
        this.ExitMenuItem = new System.Windows.Forms.MenuItem();
        this.HelpMenuItem = new System.Windows.Forms.MenuItem();
        this.HowToPlayMenuItem = new System.Windows.Forms.MenuItem();
        this.RulesMenuItem = new System.Windows.Forms.MenuItem();
        this.HelpSeparatorMenuItem = new System.Windows.Forms.MenuItem();
        this.AboutMenuItem = new System.Windows.Forms.MenuItem();
        this.StatusBar = new System.Windows.Forms.StatusBar();
        this.PromptPanel = new System.Windows.Forms.StatusBarPanel();
        this.RoundPanel = new System.Windows.Forms.StatusBarPanel();
        this.ScorePanel = new System.Windows.Forms.StatusBarPanel();
        this.Board = new HeadAndFoot.DesktopApp.GameBoard.GameBoardControl();
        ((System.ComponentModel.ISupportInitialize)(this.PromptPanel)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.RoundPanel)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.ScorePanel)).BeginInit();
        this.SuspendLayout();
        //
        // MainMenu
        //
        this.MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
        this.GameMenuItem,
        this.HelpMenuItem});
        //
        // GameMenuItem
        //
        this.GameMenuItem.Index = 0;
        this.GameMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
        this.DealMenuItem,
        this.GameSeparatorMenuItem,
        this.ExitMenuItem});
        this.GameMenuItem.Text = "&Game";
        //
        // DealMenuItem
        //
        this.DealMenuItem.Index = 0;
        this.DealMenuItem.Shortcut = System.Windows.Forms.Shortcut.F2;
        this.DealMenuItem.Text = "&Deal";
        this.DealMenuItem.Click += new System.EventHandler(this.DealMenuItem_Click);
        //
        // GameSeparatorMenuItem
        //
        this.GameSeparatorMenuItem.Index = 1;
        this.GameSeparatorMenuItem.Text = "-";
        //
        // ExitMenuItem
        //
        this.ExitMenuItem.Index = 2;
        this.ExitMenuItem.Text = "E&xit";
        this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
        //
        // HelpMenuItem
        //
        this.HelpMenuItem.Index = 1;
        this.HelpMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
        this.HowToPlayMenuItem,
        this.RulesMenuItem,
        this.HelpSeparatorMenuItem,
        this.AboutMenuItem});
        this.HelpMenuItem.Text = "&Help";
        //
        // HowToPlayMenuItem
        //
        this.HowToPlayMenuItem.Index = 0;
        this.HowToPlayMenuItem.Shortcut = System.Windows.Forms.Shortcut.F1;
        this.HowToPlayMenuItem.Text = "&How to Play";
        this.HowToPlayMenuItem.Click += new System.EventHandler(this.HowToPlayMenuItem_Click);
        //
        // RulesMenuItem
        //
        this.RulesMenuItem.Index = 1;
        this.RulesMenuItem.Text = "&Rules on the Web";
        this.RulesMenuItem.Click += new System.EventHandler(this.RulesMenuItem_Click);
        //
        // HelpSeparatorMenuItem
        //
        this.HelpSeparatorMenuItem.Index = 2;
        this.HelpSeparatorMenuItem.Text = "-";
        //
        // AboutMenuItem
        //
        this.AboutMenuItem.Index = 3;
        this.AboutMenuItem.Text = "&About Head and Foot";
        this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
        //
        // StatusBar
        //
        this.StatusBar.Location = new System.Drawing.Point(0, 500);
        this.StatusBar.Name = "StatusBar";
        this.StatusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
        this.PromptPanel,
        this.RoundPanel,
        this.ScorePanel});
        this.StatusBar.ShowPanels = true;
        this.StatusBar.Size = new System.Drawing.Size(784, 22);
        this.StatusBar.TabIndex = 1;
        //
        // PromptPanel
        //
        this.PromptPanel.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
        this.PromptPanel.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
        this.PromptPanel.Name = "PromptPanel";
        this.PromptPanel.Width = 434;
        //
        // RoundPanel
        //
        this.RoundPanel.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
        this.RoundPanel.Name = "RoundPanel";
        this.RoundPanel.Width = 150;
        //
        // ScorePanel
        //
        this.ScorePanel.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
        this.ScorePanel.Name = "ScorePanel";
        this.ScorePanel.Width = 180;
        //
        // Board
        //
        this.Board.BackColor = System.Drawing.Color.Green;
        this.Board.Dock = System.Windows.Forms.DockStyle.Fill;
        this.Board.Location = new System.Drawing.Point(0, 0);
        this.Board.Name = "Board";
        this.Board.Size = new System.Drawing.Size(784, 500);
        this.Board.TabIndex = 0;
        //
        // ShellForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(784, 522);
        this.Controls.Add(this.Board);
        this.Controls.Add(this.StatusBar);
        this.KeyPreview = true;
        this.Menu = this.MainMenu;
        this.Name = "ShellForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Head and Foot";
        ((System.ComponentModel.ISupportInitialize)(this.PromptPanel)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.RoundPanel)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.ScorePanel)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.MainMenu MainMenu;
    private System.Windows.Forms.MenuItem GameMenuItem;
    private System.Windows.Forms.MenuItem DealMenuItem;
    private System.Windows.Forms.MenuItem GameSeparatorMenuItem;
    private System.Windows.Forms.MenuItem ExitMenuItem;
    private System.Windows.Forms.MenuItem HelpMenuItem;
    private System.Windows.Forms.MenuItem HowToPlayMenuItem;
    private System.Windows.Forms.MenuItem RulesMenuItem;
    private System.Windows.Forms.MenuItem HelpSeparatorMenuItem;
    private System.Windows.Forms.MenuItem AboutMenuItem;
    private System.Windows.Forms.StatusBar StatusBar;
    private System.Windows.Forms.StatusBarPanel PromptPanel;
    private System.Windows.Forms.StatusBarPanel RoundPanel;
    private System.Windows.Forms.StatusBarPanel ScorePanel;
    private HeadAndFoot.DesktopApp.GameBoard.GameBoardControl Board;
}
