using HeadAndFoot.Business.Cards;
using System.Collections.Generic;

namespace HeadAndFoot.Business.Game;

/// <summary>
/// Validates the composition of melds: one natural rank, no threes, natural cards
/// outnumbering wild cards, and no more than a book's worth of cards.
/// </summary>
public static class MeldRules
{
    /// <summary>
    /// Checks that <paramref name="cards"/> can form a new meld on their own.
    /// </summary>
    public static MoveError ValidateNew(IList<Card> cards, out Rank rank)
    {
        rank = Rank.Joker;
        if (cards.Count == 0)
        {
            return MoveError.NoCardsSelected;
        }

        if (cards.Count < GameRules.MINIMUM_MELD_SIZE)
        {
            MoveError compositionError = ValidateComposition(cards, null, out rank);
            return compositionError != MoveError.None ? compositionError : MoveError.MeldTooSmall;
        }

        return ValidateComposition(cards, null, out rank);
    }

    /// <summary>
    /// Checks that <paramref name="cards"/> can be added to <paramref name="meld"/>.
    /// </summary>
    public static MoveError ValidateAddition(Meld meld, IList<Card> cards)
    {
        if (cards.Count == 0)
        {
            return MoveError.NoCardsSelected;
        }

        if (meld.IsBook)
        {
            return MoveError.MeldFull;
        }

        return ValidateComposition(cards, meld, out _);
    }

    private static MoveError ValidateComposition(IList<Card> cards, Meld existing, out Rank rank)
    {
        rank = existing != null ? existing.Rank : Rank.Joker;
        int naturals = existing != null ? existing.NaturalCount : 0;
        int wilds = existing != null ? existing.WildCount : 0;

        foreach (Card card in cards)
        {
            if (card.IsThree)
            {
                return MoveError.ThreesCannotBeMelded;
            }

            if (card.IsWild)
            {
                wilds++;
                continue;
            }

            if (rank == Rank.Joker)
            {
                rank = card.Rank;
            }
            else if (card.Rank != rank)
            {
                return MoveError.MixedRanks;
            }

            naturals++;
        }

        if (naturals == 0)
        {
            return MoveError.NoNaturalCards;
        }

        if (naturals + wilds > GameRules.BOOK_SIZE)
        {
            return MoveError.MeldFull;
        }

        if (wilds >= naturals)
        {
            return MoveError.TooManyWilds;
        }

        return MoveError.None;
    }
}
