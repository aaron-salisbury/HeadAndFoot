using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HeadAndFoot.App.Forms
{
    public partial class DeckSelectionForm : Form
    {
        private const int IMAGE_MARGIN = 1;

        public DeckSelectionForm()
        {
            InitializeComponent();

            SetupDeckSelectionForm();
            SetupDeckPanel();
        }

        private void SetupDeckSelectionForm()
        {
            ShowIcon = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void SetupDeckPanel()
        {
            DeckTLP.Controls.Clear();
            DeckTLP.RowStyles.Clear();
            DeckTLP.ColumnStyles.Clear();
            DeckTLP.AutoSize = true;
            DeckTLP.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            DeckTLP.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);

            BuildDeckImages();
        }

        private void BuildDeckImages()
        {
            DeckTLP.SuspendLayout();

            DeckTLP.RowCount = 2;
            DeckTLP.ColumnCount = 6;

            int largestHeight = 0;
            int largestWidth = 0;

            int index = 0;
            for (int row = 0; row < DeckTLP.RowCount; row++)
            {
                for (int column = 0; column < DeckTLP.ColumnCount; column++)
                {
                    if (index < ShellForm.GameManager.CardBackSprites.Count)
                    {
                        KeyValuePair<string, Image> deckBackImageByName = ShellForm.GameManager.CardBackSprites.ElementAt(index++);

                        Button deckBack = new Button();
                        deckBack.Tag = deckBackImageByName.Key;
                        deckBack.Image = deckBackImageByName.Value;
                        deckBack.Width = deckBack.Image.Width;
                        deckBack.Height = deckBack.Image.Height;
                        deckBack.Margin = new Padding(IMAGE_MARGIN);
                        deckBack.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);

                        deckBack.Click += new EventHandler(DeckBack_OnClick);

                        DeckTLP.Controls.Add(deckBack);
                        DeckTLP.SetRow(deckBack, row);
                        DeckTLP.SetColumn(deckBack, column);

                        if (largestHeight < deckBack.Image.Height)
                        {
                            largestHeight = deckBack.Image.Height;
                        }

                        if (largestWidth < deckBack.Image.Width)
                        {
                            largestWidth = deckBack.Image.Width;
                        }
                    }
                }
            }

            SetRowAndColumnStyles(ShellForm.GameManager.CardBackSprites.Count, largestHeight, largestWidth);

            DeckTLP.ResumeLayout(true);
        }

        private void SetRowAndColumnStyles(int numElements, int height, int width)
        {
            int rowHeight = height + (IMAGE_MARGIN * 2);
            int columnWidth = width + (IMAGE_MARGIN * 2);

            for (int i = 0; i < DeckTLP.RowCount; i++)
            {
                DeckTLP.RowStyles.Add(new RowStyle(SizeType.Absolute, rowHeight));
            }

            for (int i = 0; i < DeckTLP.ColumnCount; i++)
            {
                DeckTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, columnWidth));
            }
        }

        private void DeckBack_OnClick(object sender, EventArgs e)
        {
            Button deckBack = (Button)sender;
            string deckName = (string)deckBack.Tag;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {

        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
