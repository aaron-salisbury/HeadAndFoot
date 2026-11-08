using System;
using System.Collections.Generic;

namespace HeadAndFoot.Business.Cards;

/// <summary>
/// Builds and shuffles the combined decks used for a round.
/// </summary>
public static class Shoe
{
    public static List<Card> Create(int deckCount)
    {
        if (deckCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(deckCount));
        }

        List<Card> cards = new(deckCount * 54);
        int id = 0;

        for (int deck = 0; deck < deckCount; deck++)
        {
            foreach (Suit suit in new Suit[] { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades })
            {
                for (int rank = (int)Rank.Two; rank <= (int)Rank.Ace; rank++)
                {
                    cards.Add(new Card(id++, suit, (Rank)rank));
                }
            }

            cards.Add(new Card(id++, Suit.None, Rank.Joker));
            cards.Add(new Card(id++, Suit.None, Rank.Joker));
        }

        return cards;
    }

    /// <summary>
    /// Fisher-Yates shuffle in place.
    /// </summary>
    public static void Shuffle(List<Card> cards, Random random)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card swap = cards[i];
            cards[i] = cards[j];
            cards[j] = swap;
        }
    }
}
