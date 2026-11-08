namespace HeadAndFoot.DesktopApp.Forms;

partial class RoundScoreForm
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
        this.HeadingLabel = new System.Windows.Forms.Label();
        this.ScoreList = new System.Windows.Forms.ListView();
        this.ItemColumn = new System.Windows.Forms.ColumnHeader();
        this.YouColumn = new System.Windows.Forms.ColumnHeader();
        this.ComputerColumn = new System.Windows.Forms.ColumnHeader();
        this.OkButton = new System.Windows.Forms.Button();
        this.SuspendLayout();
        //
        // HeadingLabel
        //
        this.HeadingLabel.Location = new System.Drawing.Point(12, 12);
        this.HeadingLabel.Name = "HeadingLabel";
        this.HeadingLabel.Size = new System.Drawing.Size(366, 18);
        this.HeadingLabel.TabIndex = 0;
        //
        // ScoreList
        //
        this.ScoreList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        this.ItemColumn,
        this.YouColumn,
        this.ComputerColumn});
        this.ScoreList.FullRowSelect = true;
        this.ScoreList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
        this.ScoreList.Location = new System.Drawing.Point(12, 36);
        this.ScoreList.MultiSelect = false;
        this.ScoreList.Name = "ScoreList";
        this.ScoreList.Size = new System.Drawing.Size(366, 154);
        this.ScoreList.TabIndex = 1;
        this.ScoreList.UseCompatibleStateImageBehavior = false;
        this.ScoreList.View = System.Windows.Forms.View.Details;
        //
        // ItemColumn
        //
        this.ItemColumn.Text = "";
        this.ItemColumn.Width = 180;
        //
        // YouColumn
        //
        this.YouColumn.Text = "You";
        this.YouColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        this.YouColumn.Width = 85;
        //
        // ComputerColumn
        //
        this.ComputerColumn.Text = "Computer";
        this.ComputerColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        this.ComputerColumn.Width = 85;
        //
        // OkButton
        //
        this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.OkButton.Location = new System.Drawing.Point(290, 200);
        this.OkButton.Name = "OkButton";
        this.OkButton.Size = new System.Drawing.Size(88, 23);
        this.OkButton.TabIndex = 2;
        this.OkButton.Text = "OK";
        //
        // RoundScoreForm
        //
        this.AcceptButton = this.OkButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.CancelButton = this.OkButton;
        this.ClientSize = new System.Drawing.Size(390, 235);
        this.Controls.Add(this.OkButton);
        this.Controls.Add(this.ScoreList);
        this.Controls.Add(this.HeadingLabel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "RoundScoreForm";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label HeadingLabel;
    private System.Windows.Forms.ListView ScoreList;
    private System.Windows.Forms.ColumnHeader ItemColumn;
    private System.Windows.Forms.ColumnHeader YouColumn;
    private System.Windows.Forms.ColumnHeader ComputerColumn;
    private System.Windows.Forms.Button OkButton;
}
