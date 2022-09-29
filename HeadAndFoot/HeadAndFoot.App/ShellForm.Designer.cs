
namespace HeadAndFoot.App
{
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
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ShellForm));
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.GameMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DealMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UndoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeckMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UUIDGeneratorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FlatUIColorPickerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LineSorterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RulesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainContentPanel = new System.Windows.Forms.Panel();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GameMenuItem,
            this.ToolsMenuItem,
            this.HelpMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            this.MenuStrip.Size = new System.Drawing.Size(589, 24);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // GameMenuItem
            // 
            this.GameMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DealMenuItem,
            this.UndoMenuItem,
            this.DeckMenuItem,
            this.OptionsMenuItem,
            this.ExitMenuItem});
            this.GameMenuItem.Name = "GameMenuItem";
            this.GameMenuItem.Size = new System.Drawing.Size(50, 22);
            this.GameMenuItem.Text = "&Game";
            // 
            // DealMenuItem
            // 
            this.DealMenuItem.Name = "DealMenuItem";
            this.DealMenuItem.Size = new System.Drawing.Size(180, 22);
            this.DealMenuItem.Text = "&Deal";
            this.DealMenuItem.Click += new System.EventHandler(this.DealMenuItem_Click);
            // 
            // UndoMenuItem
            // 
            this.UndoMenuItem.Enabled = false;
            this.UndoMenuItem.Name = "UndoMenuItem";
            this.UndoMenuItem.Size = new System.Drawing.Size(180, 22);
            this.UndoMenuItem.Text = "&Undo";
            // 
            // DeckMenuItem
            // 
            this.DeckMenuItem.Name = "DeckMenuItem";
            this.DeckMenuItem.Size = new System.Drawing.Size(180, 22);
            this.DeckMenuItem.Text = "De&ck...";
            // 
            // OptionsMenuItem
            // 
            this.OptionsMenuItem.Name = "OptionsMenuItem";
            this.OptionsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.OptionsMenuItem.Text = "&Options...";
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(180, 22);
            this.ExitMenuItem.Text = "E&xit";
            this.ExitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            // 
            // ToolsMenuItem
            // 
            this.ToolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UUIDGeneratorMenuItem,
            this.FlatUIColorPickerMenuItem,
            this.LineSorterMenuItem});
            this.ToolsMenuItem.Name = "ToolsMenuItem";
            this.ToolsMenuItem.Size = new System.Drawing.Size(46, 22);
            this.ToolsMenuItem.Text = "&Tools";
            // 
            // UUIDGeneratorMenuItem
            // 
            this.UUIDGeneratorMenuItem.Name = "UUIDGeneratorMenuItem";
            this.UUIDGeneratorMenuItem.Size = new System.Drawing.Size(174, 22);
            this.UUIDGeneratorMenuItem.Text = "&UUID Generator";
            this.UUIDGeneratorMenuItem.Click += new System.EventHandler(this.UUIDGeneratorMenuItem_Click);
            // 
            // FlatUIColorPickerMenuItem
            // 
            this.FlatUIColorPickerMenuItem.Name = "FlatUIColorPickerMenuItem";
            this.FlatUIColorPickerMenuItem.Size = new System.Drawing.Size(174, 22);
            this.FlatUIColorPickerMenuItem.Text = "Flat UI &Color Picker";
            this.FlatUIColorPickerMenuItem.Click += new System.EventHandler(this.FlatUIColorPickerMenuItem_Click);
            // 
            // LineSorterMenuItem
            // 
            this.LineSorterMenuItem.Name = "LineSorterMenuItem";
            this.LineSorterMenuItem.Size = new System.Drawing.Size(174, 22);
            this.LineSorterMenuItem.Text = "Line Sorter";
            this.LineSorterMenuItem.Click += new System.EventHandler(this.LineSorterMenuItem_Click);
            // 
            // HelpMenuItem
            // 
            this.HelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RulesMenuItem,
            this.LogMenuItem,
            this.AboutMenuItem});
            this.HelpMenuItem.Name = "HelpMenuItem";
            this.HelpMenuItem.Size = new System.Drawing.Size(44, 22);
            this.HelpMenuItem.Text = "&Help";
            // 
            // RulesMenuItem
            // 
            this.RulesMenuItem.Name = "RulesMenuItem";
            this.RulesMenuItem.Size = new System.Drawing.Size(180, 22);
            this.RulesMenuItem.Text = "&Rules";
            this.RulesMenuItem.Click += new System.EventHandler(this.RulesMenuItem_Click);
            // 
            // LogMenuItem
            // 
            this.LogMenuItem.Name = "LogMenuItem";
            this.LogMenuItem.Size = new System.Drawing.Size(180, 22);
            this.LogMenuItem.Text = "&Log...";
            this.LogMenuItem.Click += new System.EventHandler(this.LogMenuItem_Click);
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.Size = new System.Drawing.Size(180, 22);
            this.AboutMenuItem.Text = "&About";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            // 
            // MainContentPanel
            // 
            this.MainContentPanel.AutoSize = true;
            this.MainContentPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainContentPanel.BackColor = System.Drawing.SystemColors.Control;
            this.MainContentPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.MainContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContentPanel.Location = new System.Drawing.Point(0, 24);
            this.MainContentPanel.Name = "MainContentPanel";
            this.MainContentPanel.Size = new System.Drawing.Size(589, 383);
            this.MainContentPanel.TabIndex = 1;
            // 
            // StatusStrip
            // 
            this.StatusStrip.Location = new System.Drawing.Point(0, 385);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(589, 22);
            this.StatusStrip.TabIndex = 2;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // ShellForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 407);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.MainContentPanel);
            this.Controls.Add(this.MenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(363, 359);
            this.Name = "ShellForm";
            this.Text = "Head & Foot";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem GameMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DealMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LogMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UUIDGeneratorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FlatUIColorPickerMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LineSorterMenuItem;
        private System.Windows.Forms.Panel MainContentPanel;
        private System.Windows.Forms.ToolStripMenuItem RulesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem UndoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DeckMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OptionsMenuItem;
        private System.Windows.Forms.StatusStrip StatusStrip;
    }
}

