
namespace AmalgamateLabs.MCGE.Presentation
{
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.AppNameLabel = new System.Windows.Forms.Label();
            this.AppDescriptionLabel = new System.Windows.Forms.Label();
            this.AppLink = new System.Windows.Forms.LinkLabel();
            this.OkButton = new System.Windows.Forms.Button();
            this.OSLabel = new System.Windows.Forms.Label();
            this.CopyLabel = new System.Windows.Forms.Label();
            this.Divider = new System.Windows.Forms.Label();
            this.ByLabel = new System.Windows.Forms.Label();
            this.LogoPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // AppNameLabel
            // 
            this.AppNameLabel.AutoSize = true;
            this.AppNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.AppNameLabel.Location = new System.Drawing.Point(75, 18);
            this.AppNameLabel.Name = "AppNameLabel";
            this.AppNameLabel.Size = new System.Drawing.Size(57, 13);
            this.AppNameLabel.TabIndex = 0;
            this.AppNameLabel.Text = "App Name";
            // 
            // AppDescriptionLabel
            // 
            this.AppDescriptionLabel.AutoSize = true;
            this.AppDescriptionLabel.Location = new System.Drawing.Point(77, 168);
            this.AppDescriptionLabel.Name = "AppDescriptionLabel";
            this.AppDescriptionLabel.Size = new System.Drawing.Size(111, 13);
            this.AppDescriptionLabel.TabIndex = 4;
            this.AppDescriptionLabel.Text = "App Description Label";
            // 
            // AppLink
            // 
            this.AppLink.AutoSize = true;
            this.AppLink.Location = new System.Drawing.Point(77, 186);
            this.AppLink.Name = "AppLink";
            this.AppLink.Size = new System.Drawing.Size(113, 13);
            this.AppLink.TabIndex = 5;
            this.AppLink.TabStop = true;
            this.AppLink.Text = "Application Web Page";
            this.AppLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AppLink_LinkClicked);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkButton.Location = new System.Drawing.Point(248, 212);
            this.OkButton.Margin = new System.Windows.Forms.Padding(0);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 6;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // OSLabel
            // 
            this.OSLabel.AutoSize = true;
            this.OSLabel.Location = new System.Drawing.Point(75, 34);
            this.OSLabel.Name = "OSLabel";
            this.OSLabel.Size = new System.Drawing.Size(66, 13);
            this.OSLabel.TabIndex = 7;
            this.OSLabel.Text = "Windows 98";
            // 
            // CopyLabel
            // 
            this.CopyLabel.AutoSize = true;
            this.CopyLabel.Location = new System.Drawing.Point(75, 50);
            this.CopyLabel.Name = "CopyLabel";
            this.CopyLabel.Size = new System.Drawing.Size(67, 13);
            this.CopyLabel.TabIndex = 8;
            this.CopyLabel.Text = "Copyright (C)";
            // 
            // Divider
            // 
            this.Divider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Divider.Location = new System.Drawing.Point(78, 157);
            this.Divider.Name = "Divider";
            this.Divider.Size = new System.Drawing.Size(245, 2);
            this.Divider.TabIndex = 9;
            // 
            // ByLabel
            // 
            this.ByLabel.AutoSize = true;
            this.ByLabel.Location = new System.Drawing.Point(75, 66);
            this.ByLabel.Name = "ByLabel";
            this.ByLabel.Size = new System.Drawing.Size(21, 13);
            this.ByLabel.TabIndex = 10;
            this.ByLabel.Text = "by ";
            // 
            // LogoPictureBox
            // 
            this.LogoPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.LogoPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.LogoPictureBox.ErrorImage = null;
            this.LogoPictureBox.InitialImage = null;
            this.LogoPictureBox.Location = new System.Drawing.Point(13, 13);
            this.LogoPictureBox.Name = "LogoPictureBox";
            this.LogoPictureBox.Size = new System.Drawing.Size(32, 32);
            this.LogoPictureBox.TabIndex = 11;
            this.LogoPictureBox.TabStop = false;
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(332, 244);
            this.Controls.Add(this.LogoPictureBox);
            this.Controls.Add(this.ByLabel);
            this.Controls.Add(this.Divider);
            this.Controls.Add(this.CopyLabel);
            this.Controls.Add(this.OSLabel);
            this.Controls.Add(this.AppNameLabel);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.AppLink);
            this.Controls.Add(this.AppDescriptionLabel);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(348, 283);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(348, 283);
            this.Name = "AboutForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label AppNameLabel;
        private System.Windows.Forms.Label AppDescriptionLabel;
        private System.Windows.Forms.LinkLabel AppLink;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.Label OSLabel;
        private System.Windows.Forms.Label CopyLabel;
        private System.Windows.Forms.Label Divider;
        private System.Windows.Forms.Label ByLabel;
        private System.Windows.Forms.PictureBox LogoPictureBox;
    }
}