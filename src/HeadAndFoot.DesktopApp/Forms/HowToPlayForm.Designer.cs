namespace HeadAndFoot.DesktopApp.Forms;

partial class HowToPlayForm
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
        this.GuideTextBox = new System.Windows.Forms.TextBox();
        this.OkButton = new System.Windows.Forms.Button();
        this.SuspendLayout();
        //
        // GuideTextBox
        //
        this.GuideTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.GuideTextBox.BackColor = System.Drawing.SystemColors.Window;
        this.GuideTextBox.Location = new System.Drawing.Point(12, 12);
        this.GuideTextBox.Multiline = true;
        this.GuideTextBox.Name = "GuideTextBox";
        this.GuideTextBox.ReadOnly = true;
        this.GuideTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        this.GuideTextBox.Size = new System.Drawing.Size(456, 330);
        this.GuideTextBox.TabIndex = 0;
        //
        // OkButton
        //
        this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.OkButton.Location = new System.Drawing.Point(393, 352);
        this.OkButton.Name = "OkButton";
        this.OkButton.Size = new System.Drawing.Size(75, 23);
        this.OkButton.TabIndex = 1;
        this.OkButton.Text = "OK";
        //
        // HowToPlayForm
        //
        this.AcceptButton = this.OkButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.OkButton;
        this.ClientSize = new System.Drawing.Size(480, 387);
        this.Controls.Add(this.OkButton);
        this.Controls.Add(this.GuideTextBox);
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.MinimumSize = new System.Drawing.Size(320, 240);
        this.Name = "HowToPlayForm";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox GuideTextBox;
    private System.Windows.Forms.Button OkButton;
}
