using HeadAndFoot.Business.Cards;
using HeadAndFoot.Business.Game;
using System;
using System.Collections.Generic;

namespace HeadAndFoot.Tests;

/// <summary>
/// Helpers for building cards and arranging specific table situations.
/// </summary>
internal static class TestCards
{
    private static int s_nextId = 100000;

    public static Card C(Rank rank, Suit suit = Suit.Spades)
    {
        return new Card(s_nextId++, rank == Rank.Joker ? Suit.None : suit, rank);
    }

    public static List<Card> Many(Rank rank, int count, Suit suit = Suit.Spades)
    {
        List<Card> cards = new();
        for (int i = 0; i < count; i++)
        {
            cards.Add(C(rank, suit));
        }

        return cards;
    }

    public static List<Card> Join(params IList<Card>[] groups)
    {
        List<Card> cards = new();
        foreach (IList<Card> group in groups)
        {
            cards.AddRange(group);
        }

        return cards;
    }

    /// <summary>
    /// Creates a started game, then replaces both players' cards and the piles with known ones.
    /// </summary>
    public static GameEngine Arrange(IList<Card> humanHand, IList<Card> discardPile, int round = 1, GamePhase phase = GamePhase.Draw)
    {
        GameEngine engine = new(new Random(1));
        engine.StartNewGame();

        engine.Human.ResetForRound();
        engine.Human.FootList.AddRange(Many(Rank.Five, GameRules.FOOT_SIZE, Suit.Hearts));
        engine.Human.HandList.AddRange(humanHand);

        engine.Computer.ResetForRound();
        engine.Computer.HandList.AddRange(Many(Rank.Nine, GameRules.HAND_SIZE, Suit.Clubs));
        engine.Computer.FootList.AddRange(Many(Rank.Nine, GameRules.FOOT_SIZE, Suit.Diamonds));

        engine.StockList.Clear();
        engine.StockList.AddRange(Many(Rank.Six, 40, Suit.Clubs));
        engine.DiscardList.Clear();
        engine.DiscardList.AddRange(discardPile);

        engine.SetTurn(engine.Human, phase, round);
        return engine;
    }

    /// <summary>
    /// Adds a committed meld to a player.
    /// </summary>
    public static Meld GiveMeld(Player player, IList<Card> cards)
    {
        Rank rank = Rank.Joker;
        foreach (Card card in cards)
        {
            if (card.IsNatural)
            {
                rank = card.Rank;
            }
        }

        Meld meld = new(rank);
        meld.CardList.AddRange(cards);
        player.MeldList.Add(meld);
        player.HasOpened = true;
        return meld;
    }
}
