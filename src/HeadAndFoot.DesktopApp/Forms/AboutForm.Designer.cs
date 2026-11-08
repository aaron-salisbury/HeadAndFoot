namespace HeadAndFoot.DesktopApp.Forms;

partial class AboutForm
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

            LogoPictureBox.Image?.Dispose();
            _logo?.Dispose();
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
        this.LogoPictureBox = new System.Windows.Forms.PictureBox();
        this.AppNameLabel = new System.Windows.Forms.Label();
        this.VersionLabel = new System.Windows.Forms.Label();
        this.CopyrightLabel = new System.Windows.Forms.Label();
        this.DescriptionLabel = new System.Windows.Forms.Label();
        this.AppLink = new System.Windows.Forms.LinkLabel();
        this.AttributionsLabel = new System.Windows.Forms.Label();
        this.AttributionsTextBox = new System.Windows.Forms.TextBox();
        this.Separator = new System.Windows.Forms.Label();
        this.OkButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
        this.SuspendLayout();
        //
        // LogoPictureBox
        //
        this.LogoPictureBox.Location = new System.Drawing.Point(14, 14);
        this.LogoPictureBox.Name = "LogoPictureBox";
        this.LogoPictureBox.Size = new System.Drawing.Size(64, 64);
        this.LogoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        this.LogoPictureBox.TabIndex = 0;
        this.LogoPictureBox.TabStop = false;
        //
        // AppNameLabel
        //
        this.AppNameLabel.AutoSize = true;
        this.AppNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        this.AppNameLabel.Location = new System.Drawing.Point(92, 16);
        this.AppNameLabel.Name = "AppNameLabel";
        this.AppNameLabel.Size = new System.Drawing.Size(0, 20);
        this.AppNameLabel.TabIndex = 1;
        //
        // VersionLabel
        //
        this.VersionLabel.AutoSize = true;
        this.VersionLabel.Location = new System.Drawing.Point(93, 42);
        this.VersionLabel.Name = "VersionLabel";
        this.VersionLabel.Size = new System.Drawing.Size(0, 13);
        this.VersionLabel.TabIndex = 2;
        //
        // CopyrightLabel
        //
        this.CopyrightLabel.AutoSize = true;
        this.CopyrightLabel.Location = new System.Drawing.Point(93, 60);
        this.CopyrightLabel.Name = "CopyrightLabel";
        this.CopyrightLabel.Size = new System.Drawing.Size(130, 13);
        this.CopyrightLabel.TabIndex = 3;
        this.CopyrightLabel.Text = "Copyright © Aaron Salisbury";
        //
        // DescriptionLabel
        //
        this.DescriptionLabel.Location = new System.Drawing.Point(93, 84);
        this.DescriptionLabel.Name = "DescriptionLabel";
        this.DescriptionLabel.Size = new System.Drawing.Size(320, 30);
        this.DescriptionLabel.TabIndex = 4;
        //
        // AppLink
        //
        this.AppLink.AutoSize = true;
        this.AppLink.Location = new System.Drawing.Point(93, 118);
        this.AppLink.Name = "AppLink";
        this.AppLink.Size = new System.Drawing.Size(120, 13);
        this.AppLink.TabIndex = 5;
        this.AppLink.TabStop = true;
        this.AppLink.Text = "Project home page";
        this.AppLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AppLink_LinkClicked);
        //
        // AttributionsLabel
        //
        this.AttributionsLabel.AutoSize = true;
        this.AttributionsLabel.Location = new System.Drawing.Point(12, 146);
        this.AttributionsLabel.Name = "AttributionsLabel";
        this.AttributionsLabel.Size = new System.Drawing.Size(60, 13);
        this.AttributionsLabel.TabIndex = 6;
        this.AttributionsLabel.Text = "Attributions:";
        //
        // AttributionsTextBox
        //
        this.AttributionsTextBox.BackColor = System.Drawing.SystemColors.Window;
        this.AttributionsTextBox.Location = new System.Drawing.Point(12, 164);
        this.AttributionsTextBox.Multiline = true;
        this.AttributionsTextBox.Name = "AttributionsTextBox";
        this.AttributionsTextBox.ReadOnly = true;
        this.AttributionsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.AttributionsTextBox.Size = new System.Drawing.Size(401, 90);
        this.AttributionsTextBox.TabIndex = 7;
        //
        // Separator
        //
        this.Separator.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.Separator.Location = new System.Drawing.Point(12, 264);
        this.Separator.Name = "Separator";
        this.Separator.Size = new System.Drawing.Size(401, 2);
        this.Separator.TabIndex = 8;
        //
        // OkButton
        //
        this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.OkButton.Location = new System.Drawing.Point(338, 274);
        this.OkButton.Name = "OkButton";
        this.OkButton.Size = new System.Drawing.Size(75, 23);
        this.OkButton.TabIndex = 0;
        this.OkButton.Text = "OK";
        //
        // AboutForm
        //
        this.AcceptButton = this.OkButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.OkButton;
        this.ClientSize = new System.Drawing.Size(425, 309);
        this.Controls.Add(this.OkButton);
        this.Controls.Add(this.Separator);
        this.Controls.Add(this.AttributionsTextBox);
        this.Controls.Add(this.AttributionsLabel);
        this.Controls.Add(this.AppLink);
        this.Controls.Add(this.DescriptionLabel);
        this.Controls.Add(this.CopyrightLabel);
        this.Controls.Add(this.VersionLabel);
        this.Controls.Add(this.AppNameLabel);
        this.Controls.Add(this.LogoPictureBox);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "AboutForm";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox LogoPictureBox;
    private System.Windows.Forms.Label AppNameLabel;
    private System.Windows.Forms.Label VersionLabel;
    private System.Windows.Forms.Label CopyrightLabel;
    private System.Windows.Forms.Label DescriptionLabel;
    private System.Windows.Forms.LinkLabel AppLink;
    private System.Windows.Forms.Label AttributionsLabel;
    private System.Windows.Forms.TextBox AttributionsTextBox;
    private System.Windows.Forms.Label Separator;
    private System.Windows.Forms.Button OkButton;
}
