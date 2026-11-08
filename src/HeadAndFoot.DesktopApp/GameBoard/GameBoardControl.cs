using HeadAndFoot.Business.Cards;
using HeadAndFoot.Business.Game;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp.GameBoard;

/// <summary>
/// The green felt table. It paints the game state it is given and reports which
/// part of the table was clicked; it does not apply any rules itself.
/// </summary>
internal sealed class GameBoardControl : Control
{
    public static readonly Size MINIMUM_BOARD_SIZE = new(784, 500);

    private const int CW = CardSheet.CARD_WIDTH;
    private const int CH = CardSheet.CARD_HEIGHT;
    private const int MARGIN = 8;
    private const int PILE_GAP = 8;
    private const int CASCADE_OFFSET = 12;
    private const int DISCARD_FAN_OFFSET = 18;
    private const int DISCARD_FAN_HEIGHT = (GameRules.MAXIMUM_PICKUP - 1) * DISCARD_FAN_OFFSET;
    private const int MELD_ROW_HEIGHT = CH + (GameRules.BOOK_SIZE - 1) * CASCADE_OFFSET;
    private const int MELD_SLOT_GAP = 6;
    private const int MINIMUM_SPACING = 1;
    private const int HAND_SPACING = 24;
    private const int SELECTED_RAISE = 16;
    private const int LABEL_HEIGHT = 14;

    private static readonly Color EMPTY_SPOT = Color.FromArgb(0, 96, 0);
    private static readonly Color LABEL_COLOR = Color.White;
    private static readonly Color HIGHLIGHT_COLOR = Color.Yellow;

    private readonly List<HitRegion> _hits = new();
    private readonly List<Card> _selected = new();
    private CardSheet _cards;
    private GameEngine _engine;

    public GameBoardControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        BackColor = Color.Green;
        MinimumSize = MINIMUM_BOARD_SIZE;
    }

    /// <summary>
    /// Raised when the player clicks a part of the table.
    /// </summary>
    public event EventHandler<BoardClickEventArgs> BoardClicked;

    public void Bind(GameEngine engine, CardSheet cards)
    {
        _engine = engine;
        _cards = cards;
        Invalidate();
    }

    public void SetSelection(IList<Card> selected)
    {
        _selected.Clear();
        _selected.AddRange(selected);
        Invalidate();
    }

    /// <summary>
    /// Orders a hand the way it is displayed: naturals by rank, then black threes, then wild cards.
    /// </summary>
    public static int CompareForHand(Card a, Card b)
    {
        int compare = HandGroup(a).CompareTo(HandGroup(b));
        if (compare == 0)
        {
            compare = a.Rank.CompareTo(b.Rank);
        }

        if (compare == 0)
        {
            compare = a.Suit.CompareTo(b.Suit);
        }

        return compare != 0 ? compare : a.Id.CompareTo(b.Id);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _hits.Clear();

        if (_engine == null || _cards == null || _engine.Phase == GamePhase.NotStarted)
        {
            return;
        }

        Graphics g = e.Graphics;
        BoardLayout layout = new(ClientSize);

        DrawComputerPiles(g, layout);
        DrawStockAndDiscard(g, layout);
        DrawMeldRow(g, _engine.Computer, layout.ComputerMeldRow, false);
        DrawMeldRow(g, _engine.Human, layout.HumanMeldRow, true);
        DrawBand(g, layout);
        DrawHumanFoot(g, layout);
        DrawHand(g, layout);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Focus();

        HitRegion hit = HitTest(e.Location);
        BoardClicked?.Invoke(this, new BoardClickEventArgs(hit.Target, hit.Card, hit.Meld, e.Button, e.Clicks));
    }

    protected override void Dispose(bool disposing)
    {
        // The card sheet is owned by the form that created it.
        _cards = null;
        base.Dispose(disposing);
    }

    private HitRegion HitTest(Point point)
    {
        for (int i = _hits.Count - 1; i >= 0; i--)
        {
            if (_hits[i].Bounds.Contains(point))
            {
                return _hits[i];
            }
        }

        return new HitRegion(Rectangle.Empty, BoardTarget.None, null, null);
    }

    private void DrawComputerPiles(Graphics g, BoardLayout layout)
    {
        Player computer = _engine.Computer;
        Point hand = new(layout.LeftColumnX, MARGIN);
        Point foot = new(layout.LeftColumnX + CW + PILE_GAP, MARGIN);

        DrawFaceDownPile(g, hand, computer.Hand.Count);
        DrawLabel(g, "Hand: " + computer.Hand.Count, new Rectangle(hand.X - 4, hand.Y + CH + 3, CW + 8, LABEL_HEIGHT), _engine.CurrentPlayer == computer);

        DrawFaceDownPile(g, foot, computer.Foot.Count);
        DrawLabel(g, computer.HasPickedUpFoot ? "Foot taken" : "Foot", new Rectangle(foot.X - 4, foot.Y + CH + 3, CW + 8, LABEL_HEIGHT), false);

        DrawLabel(g, computer.Name, new Rectangle(layout.LeftColumnX, hand.Y + CH + 3 + LABEL_HEIGHT, CW * 2 + PILE_GAP, LABEL_HEIGHT), _engine.CurrentPlayer == computer);
    }

    private void DrawStockAndDiscard(Graphics g, BoardLayout layout)
    {
        Point stock = new(layout.LeftColumnX, layout.PileY);
        Point discard = new(layout.LeftColumnX + CW + PILE_GAP, layout.PileY);

        DrawFaceDownPile(g, stock, _engine.StockCount);
        _hits.Add(new HitRegion(new Rectangle(stock, new Size(CW, CH)), BoardTarget.Stock, null, null));
        DrawLabel(g, "Stock: " + _engine.StockCount, new Rectangle(stock.X - 4, stock.Y + CH + 3, CW + 8, LABEL_HEIGHT), false);

        // The cards a pickup would take (the top card and the ones beneath it) are fanned
        // downward so each one's corner index shows, with the top card lowest and whole.
        Rectangle pile = new(discard, new Size(CW, CH));
        int count = _engine.DiscardPile.Count;
        if (count == 0)
        {
            DrawEmptySpot(g, discard);
        }
        else
        {
            int shown = Math.Min(GameRules.MAXIMUM_PICKUP, count);
            List<Card> fan = new();
            for (int i = count - shown; i < count; i++)
            {
                fan.Add(_engine.DiscardPile[i]);
            }

            if (count > shown)
            {
                DrawStackEdges(g, discard, Math.Min(2, count - shown));
            }

            pile = DrawCascade(g, fan, discard.X, discard.Y, DISCARD_FAN_OFFSET);
        }

        _hits.Add(new HitRegion(pile, BoardTarget.DiscardPile, null, null));
        DrawLabel(g, "Discard: " + count, new Rectangle(discard.X - 4, pile.Bottom + 3, CW + 8, LABEL_HEIGHT), false);
    }

    private void DrawHumanFoot(Graphics g, BoardLayout layout)
    {
        Player human = _engine.Human;
        Point foot = new(layout.LeftColumnX + CW + PILE_GAP, layout.HandY);
        DrawLabel(g, human.HasPickedUpFoot ? "Foot taken" : "Your foot", new Rectangle(foot.X - 4, foot.Y - LABEL_HEIGHT - 3, CW + 8, LABEL_HEIGHT), false);
        DrawFaceDownPile(g, foot, human.Foot.Count);

        Rectangle youLabel = new(layout.LeftColumnX - 4, layout.HandY + CH / 2 - LABEL_HEIGHT / 2, CW + 8, LABEL_HEIGHT);
        DrawLabel(g, "You: " + human.Hand.Count, youLabel, _engine.CurrentPlayer == human);
    }

    private void DrawMeldRow(Graphics g, Player player, Rectangle row, bool interactive)
    {
        if (interactive)
        {
            _hits.Add(new HitRegion(row, BoardTarget.MeldArea, null, null));
        }

        List<Meld> books = new();
        List<Meld> open = new();
        foreach (Meld meld in player.Melds)
        {
            if (meld.IsBook)
            {
                books.Add(meld);
            }
            else
            {
                open.Add(meld);
            }
        }

        open.Sort(delegate(Meld a, Meld b) { return a.Rank.CompareTo(b.Rank); });

        List<Meld> pending = new();
        if (_engine.CurrentPlayer == player)
        {
            pending.AddRange(_engine.PendingMelds);
        }

        int slots = books.Count + open.Count + pending.Count;
        if (slots == 0)
        {
            if (interactive)
            {
                DrawHint(g, "Your melds go here", row);
            }

            return;
        }

        int spacing = CW + MELD_SLOT_GAP;
        if (slots > 1 && (slots - 1) * spacing + CW > row.Width)
        {
            // Squeeze as tightly as needed so every meld stays on the table and clickable.
            spacing = Math.Max(MINIMUM_SPACING, (row.Width - CW) / (slots - 1));
        }

        int x = row.X;
        foreach (Meld book in books)
        {
            DrawBook(g, book, x, row.Y);
            if (interactive)
            {
                _hits.Add(new HitRegion(new Rectangle(x, row.Y, CW, CH), BoardTarget.Meld, null, book));
            }

            x += spacing;
        }

        foreach (Meld meld in open)
        {
            Rectangle bounds = DrawCascade(g, new List<Card>(meld.Cards), x, row.Y, CASCADE_OFFSET);
            if (interactive)
            {
                _hits.Add(new HitRegion(bounds, BoardTarget.Meld, null, meld));
            }

            x += spacing;
        }

        foreach (Meld meld in pending)
        {
            Rectangle bounds = DrawCascade(g, new List<Card>(meld.Cards), x, row.Y, CASCADE_OFFSET);
            using (Pen pen = new(HIGHLIGHT_COLOR, 2f))
            {
                pen.DashStyle = DashStyle.Dash;
                g.DrawRectangle(pen, bounds.X - 3, bounds.Y - 3, bounds.Width + 5, bounds.Height + 5);
            }

            if (interactive)
            {
                _hits.Add(new HitRegion(bounds, BoardTarget.Meld, null, meld));
            }

            x += spacing;
        }
    }

    private void DrawBand(Graphics g, BoardLayout layout)
    {
        string text = _engine.LastEvent;
        if (_engine.PendingMelds.Count > 0)
        {
            text += Environment.NewLine + "Opening melds: " + _engine.PendingPoints + " of " + _engine.OpeningMinimum + " points.";
        }

        TextRenderer.DrawText(g, text, Font, layout.Band, HIGHLIGHT_COLOR, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis);
    }

    private void DrawHand(Graphics g, BoardLayout layout)
    {
        List<Card> hand = new(_engine.Human.Hand);
        hand.Sort(CompareForHand);

        if (hand.Count == 0)
        {
            return;
        }

        int available = layout.HandArea.Width;
        int spacing = HAND_SPACING;
        if (hand.Count > 1 && (hand.Count - 1) * spacing + CW > available)
        {
            // Squeeze as tightly as needed so every card stays on the table and clickable.
            spacing = Math.Max(MINIMUM_SPACING, (available - CW) / (hand.Count - 1));
        }

        int x = layout.HandArea.X;
        foreach (Card card in hand)
        {
            int y = _selected.Contains(card) ? layout.HandY - SELECTED_RAISE : layout.HandY;
            _cards.DrawFace(g, card, x, y);
            _hits.Add(new HitRegion(new Rectangle(x, y, CW, CH), BoardTarget.HandCard, card, null));
            x += spacing;
        }
    }

    private Rectangle DrawCascade(Graphics g, List<Card> cards, int x, int y, int offset)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            _cards.DrawFace(g, cards[i], x, y + i * offset);
        }

        return new Rectangle(x, y, CW, CH + Math.Max(0, cards.Count - 1) * offset);
    }

    private void DrawBook(Graphics g, Meld book, int x, int y)
    {
        // A completed book is squared up with a red card on top when clean and a black card when dirty.
        Card top = book.Cards[0];
        foreach (Card card in book.Cards)
        {
            if (card.IsNatural && card.IsRed == book.IsClean)
            {
                top = card;
                break;
            }
        }

        DrawStackEdges(g, new Point(x, y), 2);
        _cards.DrawFace(g, top, x, y);
        DrawLabel(g, book.IsClean ? "Clean book" : "Dirty book", new Rectangle(x - 6, y + CH + 6, CW + 12, LABEL_HEIGHT), book.IsClean);
    }

    private void DrawFaceDownPile(Graphics g, Point location, int count)
    {
        if (count == 0)
        {
            DrawEmptySpot(g, location);
            return;
        }

        int edges = Math.Min(2, (count - 1) / 10);
        DrawStackEdges(g, location, edges);
        _cards.DrawBack(g, location.X, location.Y);
    }

    private static void DrawStackEdges(Graphics g, Point location, int edges)
    {
        for (int i = edges; i >= 1; i--)
        {
            Rectangle edge = new(location.X + i * 2, location.Y + i * 2, CW - 1, CH - 1);
            g.FillRectangle(Brushes.White, edge);
            g.DrawRectangle(Pens.Black, edge);
        }
    }

    private static void DrawEmptySpot(Graphics g, Point location)
    {
        using Pen pen = new(EMPTY_SPOT, 2f);
        g.DrawRectangle(pen, location.X + 1, location.Y + 1, CW - 3, CH - 3);
    }

    private void DrawLabel(Graphics g, string text, Rectangle bounds, bool highlight)
    {
        TextRenderer.DrawText(g, text, Font, bounds, highlight ? HIGHLIGHT_COLOR : LABEL_COLOR, TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.SingleLine);
    }

    private void DrawHint(Graphics g, string text, Rectangle bounds)
    {
        TextRenderer.DrawText(g, text, Font, bounds, Color.FromArgb(160, 208, 160), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
    }

    private static int HandGroup(Card card)
    {
        if (card.IsWild)
        {
            return 2;
        }

        return card.IsThree ? 1 : 0;
    }

    /// <summary>
    /// Positions of each area of the table for the current size.
    /// </summary>
    private sealed class BoardLayout
    {
        public BoardLayout(Size size)
        {
            int width = Math.Max(size.Width, MINIMUM_BOARD_SIZE.Width);
            int height = Math.Max(size.Height, MINIMUM_BOARD_SIZE.Height);

            LeftColumnX = MARGIN;
            int rightX = MARGIN + CW * 2 + PILE_GAP + 16;
            int rightWidth = width - rightX - MARGIN;

            HandY = height - MARGIN - CH;
            HandArea = new Rectangle(rightX, HandY, rightWidth, CH);
            ComputerMeldRow = new Rectangle(rightX, MARGIN, rightWidth, MELD_ROW_HEIGHT);
            HumanMeldRow = new Rectangle(rightX, HandY - SELECTED_RAISE - 6 - MELD_ROW_HEIGHT, rightWidth, MELD_ROW_HEIGHT);
            Band = Rectangle.FromLTRB(rightX, ComputerMeldRow.Bottom + 2, rightX + rightWidth, HumanMeldRow.Top - 2);

            int computerLabelsBottom = MARGIN + CH + 3 + LABEL_HEIGHT * 2;
            int humanLabelsTop = HandY - LABEL_HEIGHT - 3;
            PileY = (computerLabelsBottom + humanLabelsTop) / 2 - (CH + DISCARD_FAN_HEIGHT + LABEL_HEIGHT) / 2;
        }

        public int LeftColumnX { get; }

        public int PileY { get; }

        public int HandY { get; }

        public Rectangle HandArea { get; }

        public Rectangle ComputerMeldRow { get; }

        public Rectangle HumanMeldRow { get; }

        public Rectangle Band { get; }
    }

    private sealed class HitRegion
    {
        public HitRegion(Rectangle bounds, BoardTarget target, Card card, Meld meld)
        {
            Bounds = bounds;
            Target = target;
            Card = card;
            Meld = meld;
        }

        public Rectangle Bounds { get; }

        public BoardTarget Target { get; }

        public Card Card { get; }

        public Meld Meld { get; }
    }
}

internal enum BoardTarget
{
    None,
    Stock,
    DiscardPile,
    HandCard,
    Meld,
    MeldArea
}

internal sealed class BoardClickEventArgs : EventArgs
{
    public BoardClickEventArgs(BoardTarget target, Card card, Meld meld, MouseButtons button, int clicks)
    {
        Target = target;
        Card = card;
        Meld = meld;
        Button = button;
        Clicks = clicks;
    }

    public BoardTarget Target { get; }

    public Card Card { get; }

    public Meld Meld { get; }

    public MouseButtons Button { get; }

    public int Clicks { get; }
}
