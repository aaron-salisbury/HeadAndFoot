using HeadAndFoot.Business.Game;
using HeadAndFoot.DesktopApp.Forms;
using HeadAndFoot.DesktopApp.GameBoard;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp;

/// <summary>
/// The main window: menu, green table, and status bar.
/// </summary>
internal partial class ShellForm : Form
{
    private readonly CardSheet _cards;
    private readonly GamePresenter _presenter;

    public ShellForm(GameEngine engine)
    {
        if (engine == null)
        {
            throw new ArgumentNullException(nameof(engine));
        }

        InitializeComponent();
        Text = AppResources.PRODUCT_NAME;
        Icon = AppResources.LoadIcon(AppResources.APP_ICON);

        _cards = new CardSheet();
        Board.Bind(engine, _cards);

        _presenter = new GamePresenter(Board, engine);
        _presenter.StatusChanged += Presenter_StatusChanged;
        _presenter.RoundEnded += Presenter_RoundEnded;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        MinimumSize = Size;
        _presenter.Deal();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Escape)
        {
            _presenter.ClearSelection();
            e.Handled = true;
        }
    }

    private void DisposeGame()
    {
        _presenter?.Dispose();
        _cards?.Dispose();
    }

    private void Presenter_StatusChanged(object sender, EventArgs e)
    {
        GameEngine engine = _presenter.Engine;
        PromptPanel.Text = _presenter.Prompt;
        RoundPanel.Text = "Round " + engine.Round + " of " + GameRules.ROUND_COUNT + "   Open: " + engine.OpeningMinimum;
        ScorePanel.Text = "You: " + engine.Human.TotalScore + "   Computer: " + engine.Computer.TotalScore;
        Board.Invalidate();
    }

    private void Presenter_RoundEnded(object sender, EventArgs e)
    {
        // Let the table repaint its final state before the score sheet appears.
        BeginInvoke(new MethodInvoker(ShowRoundResults));
    }

    private void ShowRoundResults()
    {
        GameEngine engine = _presenter.Engine;
        if (engine.Phase != GamePhase.RoundOver && engine.Phase != GamePhase.GameOver)
        {
            // A new deal was started before this queued callback ran; its results no longer apply.
            return;
        }

        using (RoundScoreForm form = new(engine))
        {
            form.ShowDialog(this);
        }

        if (engine.Phase == GamePhase.RoundOver)
        {
            _presenter.ContinueToNextRound();
            return;
        }

        Player winner = engine.Winner;
        string result = winner == null
            ? "The game is a tie at " + engine.Human.TotalScore + " points."
            : winner == engine.Human
                ? "You win, " + engine.Human.TotalScore + " to " + engine.Computer.TotalScore + "!"
                : "The computer wins, " + engine.Computer.TotalScore + " to " + engine.Human.TotalScore + ".";

        if (MessageBox.Show(this, result + Environment.NewLine + Environment.NewLine + "Deal again?", AppResources.PRODUCT_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
        {
            _presenter.Deal();
        }
    }

    private void DealMenuItem_Click(object sender, EventArgs e)
    {
        if (_presenter.IsGameInProgress && MessageBox.Show(this, "Abandon this game and deal a new one?", AppResources.PRODUCT_NAME, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        _presenter.Deal();
    }

    private void ExitMenuItem_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void HowToPlayMenuItem_Click(object sender, EventArgs e)
    {
        using HowToPlayForm form = new();
        form.ShowDialog(this);
    }

    private void RulesMenuItem_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start(AppResources.RULES_LINK);
        }
        catch (System.ComponentModel.Win32Exception)
        {
            MessageBox.Show(this, "The rules are available at:" + Environment.NewLine + AppResources.RULES_LINK, AppResources.PRODUCT_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void AboutMenuItem_Click(object sender, EventArgs e)
    {
        using AboutForm form = new();
        form.ShowDialog(this);
    }
}
