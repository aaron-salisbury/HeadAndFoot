using HeadAndFoot.Business.Cards;
using HeadAndFoot.Business.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static HeadAndFoot.Tests.TestCards;

namespace HeadAndFoot.Tests;

[TestClass]
public class ComputerPlayerTests
{
    [TestMethod]
    public void ComputerOpensWhenItCan()
    {
        GameEngine engine = Arrange(Join(Many(Rank.Ace, 3), new[] { C(Rank.Four), C(Rank.Seven) }), new[] { C(Rank.King) }, 1, GamePhase.Play);
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();

        Assert.IsTrue(engine.Human.HasOpened);
        Assert.AreEqual(1, engine.Human.Melds.Count);
    }

    [TestMethod]
    public void ComputerDoesNotStageAnOpeningBelowMinimum()
    {
        GameEngine engine = Arrange(Join(Many(Rank.Four, 3), new[] { C(Rank.Nine), C(Rank.King) }), new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();

        Assert.AreEqual(0, engine.PendingMelds.Count);
        Assert.AreSame(engine.Computer, engine.CurrentPlayer, "Having nothing to meld, it should have discarded.");
    }

    [TestMethod]
    public void ComputerDiscardsBlackThreeFirst()
    {
        Card blackThree = C(Rank.Three, Suit.Clubs);
        GameEngine engine = Arrange(Join(new[] { C(Rank.Nine), blackThree, C(Rank.Five) }), new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();

        Assert.AreSame(blackThree, engine.TopDiscard);
    }

    [TestMethod]
    public void ComputerGoesOutWhenItCan()
    {
        GameEngine engine = Arrange(Many(Rank.Nine, 3), new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        engine.Human.HasPickedUpFoot = true;
        engine.Human.FootList.Clear();
        GiveMeld(engine.Human, Many(Rank.Ace, 7));
        GiveMeld(engine.Human, Join(Many(Rank.King, 4), Many(Rank.Two, 3)));
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();

        Assert.AreSame(engine.Human, engine.WentOut);
        Assert.AreEqual(GamePhase.RoundOver, engine.Phase);
    }

    [TestMethod]
    public void ComputerKeepsACardRatherThanDiscardingItsLast()
    {
        Card nine = C(Rank.Nine);
        GameEngine engine = Arrange(new[] { nine, C(Rank.Four) }, new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        engine.Human.HasPickedUpFoot = true;
        engine.Human.FootList.Clear();
        GiveMeld(engine.Human, Many(Rank.Ace, 7));
        GiveMeld(engine.Human, Join(Many(Rank.King, 4), Many(Rank.Two, 3)));
        GiveMeld(engine.Human, Many(Rank.Nine, 4));
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();

        Assert.AreEqual(GamePhase.Draw, engine.Phase);
        Assert.AreSame(engine.Computer, engine.CurrentPlayer);
        Assert.AreEqual(1, engine.Human.Hand.Count);
    }

    [TestMethod]
    public void ComputerOpensWithWholeHandWhenPickupReleasesBuriedCards()
    {
        List<Card> pile = Join(new[] { C(Rank.Four), C(Rank.Five) }, new[] { C(Rank.Ace) });
        GameEngine engine = Arrange(Join(Many(Rank.Ace, 2), Many(Rank.King, 3)), pile, 2);
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();
        Assert.IsTrue(engine.HasPendingPickup, "The pickup should be kept toward the opening.");

        computer.TakeNextAction();

        Assert.IsTrue(engine.Human.HasOpened);
        Assert.AreEqual(2, engine.Human.Hand.Count);
        Assert.AreEqual(0, engine.DiscardPile.Count);
    }

    [TestMethod]
    public void ComputerOpensWithItsWholeHandAndTakesItsFoot()
    {
        GameEngine engine = Arrange(Join(Many(Rank.Ace, 3), Many(Rank.King, 3)), new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();
        computer.TakeNextAction();

        Assert.IsTrue(engine.Human.HasOpened);
        Assert.IsTrue(engine.Human.HasPickedUpFoot);
        Assert.AreEqual(GameRules.FOOT_SIZE, engine.Human.Hand.Count);
        Assert.AreSame(engine.Human, engine.CurrentPlayer);
    }

    [TestMethod]
    public void ComputerGoesOutWithAnOpeningThatUsesItsWholeFoot()
    {
        List<Card> hand = Join(Many(Rank.Four, 7), Many(Rank.Five, 5), Many(Rank.Two, 2), Many(Rank.Ace, 3));
        GameEngine engine = Arrange(hand, new[] { C(Rank.Queen) }, 4, GamePhase.Play);
        engine.Human.HasPickedUpFoot = true;
        engine.Human.FootList.Clear();
        ComputerPlayer computer = new(engine, engine.Human);

        for (int i = 0; i < 6 && engine.IsRoundInProgress; i++)
        {
            computer.TakeNextAction();
        }

        Assert.AreSame(engine.Human, engine.WentOut);
    }

    [TestMethod]
    public void ComputerFallsBackWhenAnOpeningWouldEmptyItsFootWithoutBooks()
    {
        GameEngine engine = Arrange(Join(Many(Rank.Ace, 3), Many(Rank.King, 3)), new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        engine.Human.HasPickedUpFoot = true;
        engine.Human.FootList.Clear();
        ComputerPlayer computer = new(engine, engine.Human);

        for (int i = 0; i < 6 && engine.CurrentPlayer == engine.Human; i++)
        {
            computer.TakeNextAction();
        }

        Assert.IsTrue(engine.IsRoundInProgress);
        Assert.AreSame(engine.Computer, engine.CurrentPlayer);
        Assert.IsTrue(engine.Human.Hand.Count >= 1);
    }

    [TestMethod]
    public void ComputerDiscardsRedThreesFirst()
    {
        Card redThree = C(Rank.Three, Suit.Hearts);
        GameEngine engine = Arrange(new[] { C(Rank.Nine), C(Rank.Three, Suit.Clubs), redThree, C(Rank.Five) }, new[] { C(Rank.Queen) }, 1, GamePhase.Play);
        ComputerPlayer computer = new(engine, engine.Human);

        computer.TakeNextAction();

        Assert.AreSame(redThree, engine.TopDiscard);
    }

    [TestMethod]
    public void ComputerVersusComputerGamesFinishAndConserveCards()
    {
        for (int seed = 0; seed < 40; seed++)
        {
            GameEngine engine = new(new Random(seed));
            ComputerPlayer first = new(engine, engine.Human);
            ComputerPlayer second = new(engine, engine.Computer);
            engine.StartNewGame();

            int actions = 0;
            while (engine.Phase != GamePhase.GameOver)
            {
                if (engine.Phase == GamePhase.RoundOver)
                {
                    engine.StartNextRound();
                    continue;
                }

                first.TakeNextAction();
                second.TakeNextAction();
                AssertCardsConserved(engine, seed);

                actions++;
                Assert.IsTrue(actions < 20000, "Game with seed " + seed + " did not finish.");
            }

            Assert.AreEqual(GameRules.ROUND_COUNT, engine.Human.RoundScores.Count);
            Assert.AreEqual(GameRules.ROUND_COUNT, engine.Computer.RoundScores.Count);
        }
    }

    private static void AssertCardsConserved(GameEngine engine, int seed)
    {
        Dictionary<int, bool> seen = new();
        int count = 0;

        void Add(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                Assert.IsFalse(seen.ContainsKey(card.Id), "Card counted twice with seed " + seed + ": " + card);
                seen[card.Id] = true;
                count++;
            }
        }

        Add(engine.DiscardPile);
        Add(engine.StockList);
        Add(engine.PendingPickupBuriedList);
        foreach (Meld meld in engine.PendingMelds)
        {
            Add(meld.Cards);
        }

        foreach (Player player in engine.Players)
        {
            Add(player.Hand);
            Add(player.Foot);
            foreach (Meld meld in player.Melds)
            {
                Add(meld.Cards);
                Assert.IsTrue(meld.Count <= GameRules.BOOK_SIZE);
                Assert.IsTrue(meld.WildCount < meld.NaturalCount);
            }
        }

        Assert.AreEqual(GameRules.DECK_COUNT * 54, count, "Card count with seed " + seed);
    }
}
