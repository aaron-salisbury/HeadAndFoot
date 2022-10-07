using System.Drawing;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace HeadAndFoot.App.Forms
{
    public partial class DeckUC : UserControl
    {
        public DeckUC()
        {
            InitializeComponent();

            SetupDeckPanel();
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

            SetRowAndColumnStyles();

            int index = 0;
            for (int row = 0; row < DeckTLP.RowCount; row++)
            {
                for (int column = 0; column < DeckTLP.ColumnCount; column++)
                {
                    if (index < ShellForm.GameManager.CardBackSprites.Count)
                    {
                        KeyValuePair<string, Image> deckBackImageByName = ShellForm.GameManager.CardBackSprites.ElementAt(index++);

                        PictureBox deckBack = new PictureBox();
                        deckBack.Tag = deckBackImageByName.Key;
                        deckBack.Image = deckBackImageByName.Value;
                        deckBack.Width = deckBack.Image.Width;
                        deckBack.Height = deckBack.Image.Height;
                        deckBack.Margin = new Padding(1);
                        deckBack.Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom);

                        //Button deckBack = new Button()
                        //{
                        //    Image = deckBackImageByName.Value,
                        //    Tag = deckBackImageByName.Key,
                        //    Margin = new Padding(1),
                        //    Anchor = (AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom)
                        //};

                        deckBack.Click += new EventHandler(DeckBack_OnClick);

                        DeckTLP.Controls.Add(deckBack);
                        DeckTLP.SetRow(deckBack, row);
                        DeckTLP.SetColumn(deckBack, column);
                    }
                }
            }

            DeckTLP.ResumeLayout(true);
        }

        private void SetRowAndColumnStyles()
        {
            float rowRercent = 100F / DeckTLP.RowCount;
            float columnRercent = 100F / DeckTLP.ColumnCount;

            for (int i = 0; i < DeckTLP.RowCount; i++)
            {
                DeckTLP.RowStyles.Add(new RowStyle(SizeType.Percent, rowRercent));
            }

            for (int i = 0; i < DeckTLP.ColumnCount; i++)
            {
                DeckTLP.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, columnRercent));
            }
        }

        private void DeckBack_OnClick(object sender, EventArgs e)
        {
            PictureBox deckBack = (PictureBox)sender;
            //Button deckBack = (Button)sender;
            string deckName = (string)deckBack.Tag;
        }
    }
}
