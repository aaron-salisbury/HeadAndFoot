using HeadAndFoot.Business.Cards;
using System;
using System.Drawing;
using System.IO;

namespace HeadAndFoot.DesktopApp.GameBoard;

/// <summary>
/// Draws cards from the embedded sprite sheet produced by tools/CardArt/New-CardArt.ps1.
/// </summary>
internal sealed class CardSheet : IDisposable
{
    public const int CARD_WIDTH = 71;
    public const int CARD_HEIGHT = 96;

    private const int JOKER_ROW = 4;
    private const int RED_JOKER_COLUMN = 0;
    private const int BLACK_JOKER_COLUMN = 1;
    private const int BACK_COLUMN = 2;

    private readonly Bitmap _sheet;

    public CardSheet()
    {
        using Stream stream = AppResources.Open(AppResources.CARD_SHEET);
        using Bitmap loaded = new(stream);

        // Copy so the bitmap no longer depends on the resource stream.
        _sheet = new Bitmap(loaded);
    }

    public void DrawFace(Graphics graphics, Card card, int x, int y)
    {
        graphics.DrawImage(_sheet, new Rectangle(x, y, CARD_WIDTH, CARD_HEIGHT), SourceRectangle(card), GraphicsUnit.Pixel);
    }

    public void DrawBack(Graphics graphics, int x, int y)
    {
        graphics.DrawImage(_sheet, new Rectangle(x, y, CARD_WIDTH, CARD_HEIGHT), Cell(JOKER_ROW, BACK_COLUMN), GraphicsUnit.Pixel);
    }

    public void Dispose()
    {
        _sheet.Dispose();
    }

    private static Rectangle SourceRectangle(Card card)
    {
        if (card.IsJoker)
        {
            // Each deck has one red and one black joker; ids alternate between them.
            return Cell(JOKER_ROW, card.Id % 2 == 0 ? RED_JOKER_COLUMN : BLACK_JOKER_COLUMN);
        }

        int row;
        switch (card.Suit)
        {
            case Suit.Clubs:
                row = 0;
                break;
            case Suit.Diamonds:
                row = 1;
                break;
            case Suit.Hearts:
                row = 2;
                break;
            default:
                row = 3;
                break;
        }

        int column = card.Rank == Rank.Ace ? 0 : (int)card.Rank - 1;
        return Cell(row, column);
    }

    private static Rectangle Cell(int row, int column)
    {
        return new Rectangle(column * CARD_WIDTH, row * CARD_HEIGHT, CARD_WIDTH, CARD_HEIGHT);
    }
}
