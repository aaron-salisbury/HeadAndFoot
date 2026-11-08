using HeadAndFoot.Business.Cards;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeadAndFoot.Business.Game;

/// <summary>
/// Three or more cards of one natural rank, possibly including wild cards.
/// A meld of seven cards is a completed book.
/// </summary>
public sealed class Meld
{
    private readonly List<Card> _cards = new();

    internal Meld(Rank rank)
    {
        Rank = rank;
        Cards = _cards.AsReadOnly();
    }

    public Rank Rank { get; }

    public ReadOnlyCollection<Card> Cards { get; }

    public int Count => _cards.Count;

    public int WildCount
    {
        get
        {
            int count = 0;
            foreach (Card card in _cards)
            {
                if (card.IsWild)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int NaturalCount => _cards.Count - WildCount;

    public bool IsBook => _cards.Count >= GameRules.BOOK_SIZE;

    public bool IsClean => WildCount == 0;

    public int CardPoints
    {
        get
        {
            int points = 0;
            foreach (Card card in _cards)
            {
                points += card.PointValue;
            }

            return points;
        }
    }

    public int BookBonus
    {
        get
        {
            if (!IsBook)
            {
                return 0;
            }

            return IsClean ? GameRules.CLEAN_BOOK_BONUS : GameRules.DIRTY_BOOK_BONUS;
        }
    }

    internal List<Card> CardList => _cards;
}
