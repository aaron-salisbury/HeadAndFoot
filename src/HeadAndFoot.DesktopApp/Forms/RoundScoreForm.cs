using HeadAndFoot.Business.Game;
using System;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp.Forms;

/// <summary>
/// The score sheet shown at the end of each round.
/// </summary>
internal partial class RoundScoreForm : Form
{
    public RoundScoreForm(GameEngine engine)
    {
        if (engine == null)
        {
            throw new ArgumentNullException(nameof(engine));
        }

        InitializeComponent();
        Text = AppResources.PRODUCT_NAME;

        RoundScore human = engine.Human.RoundScores[engine.Human.RoundScores.Count - 1];
        RoundScore computer = engine.Computer.RoundScores[engine.Computer.RoundScores.Count - 1];

        string ending = engine.WentOut == null
            ? "The stock ran out."
            : engine.WentOut == engine.Human ? "You went out." : "The computer went out.";
        HeadingLabel.Text = "Round " + human.Round + " of " + GameRules.ROUND_COUNT + " is over. " + ending;

        AddRow("Melded cards", human.MeldPoints, computer.MeldPoints);
        AddRow("Book bonuses", human.BookBonus, computer.BookBonus);
        AddRow("Going out", human.GoingOutBonus, computer.GoingOutBonus);
        AddRow("Cards left in hand and foot", -human.CardsLeftPenalty, -computer.CardsLeftPenalty);
        AddRow("Round total", human.Total, computer.Total);
        AddRow("Game total", engine.Human.TotalScore, engine.Computer.TotalScore);

        OkButton.Text = engine.Phase == GamePhase.GameOver ? "OK" : "Next Round";
    }

    private void AddRow(string label, int human, int computer)
    {
        ListViewItem item = new(label);
        item.SubItems.Add(human.ToString());
        item.SubItems.Add(computer.ToString());
        ScoreList.Items.Add(item);
    }
}
