using HeadAndFoot.Business.Cards;
using HeadAndFoot.Business.Game;
using System;
using System.Collections.Generic;
using System.Media;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp.GameBoard;

/// <summary>
/// Turns clicks on the table into game moves, keeps the player's card selection,
/// and paces the computer's turn so each of its actions can be seen.
/// </summary>
internal sealed class GamePresenter : IDisposable
{
    private const int COMPUTER_STEP_INTERVAL = 750;

    private readonly GameBoardControl _board;
    private readonly GameEngine _engine;
    private readonly ComputerPlayer _computer;
    private readonly Timer _computerTimer;
    private readonly List<Card> _selected = new();

    public GamePresenter(GameBoardControl board, GameEngine engine)
    {
        if (board == null)
        {
            throw new ArgumentNullException(nameof(board));
        }

        if (engine == null)
        {
            throw new ArgumentNullException(nameof(engine));
        }

        _board = board;
        _engine = engine;
        _computer = new ComputerPlayer(engine, engine.Computer);
        _computerTimer = new Timer { Interval = COMPUTER_STEP_INTERVAL };
        _computerTimer.Tick += ComputerTimer_Tick;
        _board.BoardClicked += Board_BoardClicked;
    }

    /// <summary>
    /// Raised when the prompt or score display should change.
    /// </summary>
    public event EventHandler StatusChanged;

    /// <summary>
    /// Raised when a round ends, after the table shows the final state.
    /// </summary>
    public event EventHandler RoundEnded;

    public string Prompt { get; private set; } = string.Empty;

    public GameEngine Engine => _engine;

    public bool IsGameInProgress => _engine.IsRoundInProgress;

    public void Deal()
    {
        _computerTimer.Stop();
        _selected.Clear();
        _engine.StartNewGame();
        BeginRound();
    }

    public void ContinueToNextRound()
    {
        _engine.StartNextRound();
        BeginRound();
    }

    public void ClearSelection()
    {
        if (_selected.Count > 0)
        {
            _selected.Clear();
            Refresh();
        }
    }

    public void Dispose()
    {
        _computerTimer.Stop();
        _computerTimer.Tick -= ComputerTimer_Tick;
        _computerTimer.Dispose();
        _board.BoardClicked -= Board_BoardClicked;
    }

    private void BeginRound()
    {
        _selected.Clear();
        Refresh();
        StartComputerIfNeeded();
    }

    private void Board_BoardClicked(object sender, BoardClickEventArgs e)
    {
        if (!IsHumanTurn())
        {
            return;
        }

        if (e.Button == MouseButtons.Right)
        {
            ClearSelection();
            return;
        }

        if (e.Button != MouseButtons.Left)
        {
            return;
        }

        switch (e.Target)
        {
            case BoardTarget.Stock:
                Apply(_engine.DrawFromStock());
                break;
            case BoardTarget.DiscardPile:
                DiscardPileClicked();
                break;
            case BoardTarget.HandCard:
                HandCardClicked(e.Card, e.Clicks);
                break;
            case BoardTarget.Meld:
                MeldClicked(e.Meld);
                break;
            case BoardTarget.MeldArea:
                if (_selected.Count > 0)
                {
                    Apply(_engine.Meld(new List<Card>(_selected), null));
                }

                break;
        }
    }

    private void DiscardPileClicked()
    {
        if (_engine.Phase == GamePhase.Draw)
        {
            Apply(_engine.PickUpDiscardPile());
            return;
        }

        if (_selected.Count != 1)
        {
            ShowError("Select exactly one card, then click the discard pile to discard it.");
            return;
        }

        Apply(_engine.Discard(_selected[0]));
    }

    private void HandCardClicked(Card card, int clicks)
    {
        if (clicks >= 2 && _engine.Phase == GamePhase.Play)
        {
            // Double-clicking a card discards it, a shortcut in the spirit of the classic games.
            Apply(_engine.Discard(card));
            return;
        }

        if (_selected.Contains(card))
        {
            _selected.Remove(card);
        }
        else
        {
            _selected.Add(card);
        }

        Refresh();
    }

    private void MeldClicked(Meld meld)
    {
        if (_selected.Count > 0)
        {
            Apply(_engine.Meld(new List<Card>(_selected), meld));
            return;
        }

        bool isPending = false;
        foreach (Meld pending in _engine.PendingMelds)
        {
            if (pending == meld)
            {
                isPending = true;
            }
        }

        if (isPending)
        {
            Apply(_engine.ReturnPendingMeld(meld));
        }
        else
        {
            ShowError("Select cards from your hand, then click a meld to add them to it.");
        }
    }

    private void Apply(MoveError error)
    {
        if (error != MoveError.None)
        {
            ShowError(MoveErrors.Describe(error));
            return;
        }

        _selected.Clear();
        Refresh();

        if (!_engine.IsRoundInProgress)
        {
            RoundEnded?.Invoke(this, EventArgs.Empty);
            return;
        }

        StartComputerIfNeeded();
    }

    private void StartComputerIfNeeded()
    {
        if (_computer.IsTurn)
        {
            _computerTimer.Start();
        }
    }

    private void ComputerTimer_Tick(object sender, EventArgs e)
    {
        if (!_computer.IsTurn)
        {
            _computerTimer.Stop();
            return;
        }

        _computer.TakeNextAction();
        Refresh();

        if (!_engine.IsRoundInProgress)
        {
            _computerTimer.Stop();
            RoundEnded?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (!_computer.IsTurn)
        {
            _computerTimer.Stop();
        }
    }

    private bool IsHumanTurn()
    {
        return _engine.IsRoundInProgress && _engine.CurrentPlayer == _engine.Human && !_computerTimer.Enabled;
    }

    private void ShowError(string message)
    {
        SystemSounds.Beep.Play();
        Prompt = message;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Refresh()
    {
        // Drop any selected card that has left the hand.
        for (int i = _selected.Count - 1; i >= 0; i--)
        {
            if (!_engine.Human.Hand.Contains(_selected[i]))
            {
                _selected.RemoveAt(i);
            }
        }

        _board.SetSelection(_selected);
        Prompt = BuildPrompt();
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private string BuildPrompt()
    {
        if (!_engine.IsRoundInProgress)
        {
            return _engine.Phase == GamePhase.GameOver ? "Game over. Press F2 to deal a new game." : "Round over.";
        }

        if (_engine.CurrentPlayer != _engine.Human)
        {
            return "The computer is playing...";
        }

        if (_engine.Phase == GamePhase.Draw)
        {
            return "Your turn: click the stock to draw two cards, or the discard pile to pick it up.";
        }

        if (_engine.HasPendingPickup)
        {
            return "Lay down " + (_engine.OpeningMinimum - _engine.PendingPoints) + " more points to open, or click the outlined meld to put the pile back.";
        }

        if (_selected.Count == 0)
        {
            return "Select cards to meld, then click your meld area or a meld. Discard one card to end your turn.";
        }

        return _selected.Count + " selected: click your meld area for a new meld, a meld to add to it, or the discard pile to discard.";
    }
}
