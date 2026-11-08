using HeadAndFoot.Business.Cards;
using HeadAndFoot.Business.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static HeadAndFoot.Tests.TestCards;

namespace HeadAndFoot.Tests;

[TestClass]
public class GameEngineTests
{
    [TestMethod]
    public void NewGameDealsHandsAndFeet()
    {
        GameEngine engine = new(new Random(7));
        engine.StartNewGame();

        Assert.AreEqual(1, engine.Round);
        Assert.AreEqual(GamePhase.Draw, engine.Phase);
        Assert.AreSame(engine.Human, engine.CurrentPlayer);

        int total = engine.StockCount + engine.DiscardPile.Count;
        foreach (Player player in engine.Players)
        {
            Assert.AreEqual(GameRules.HAND_SIZE, player.Hand.Count);
            Assert.AreEqual(GameRules.FOOT_SIZE, player.Foot.Count);
            total += player.Hand.Count + player.Foot.Count;
        }

        Assert.AreEqual(GameRules.DECK_COUNT * 54, total);
        Assert.IsTrue(engine.TopDiscard.IsNatural || engine.StockCount == 0);
    }

    [TestMethod]
    public void DrawingTakesTwoCards()
    {
        GameEngine engine = Arrange(Many(Rank.Four, 5), new[] { C(Rank.King) });

        Assert.AreEqual(MoveError.None, engine.DrawFromStock());
        Assert.AreEqual(7, engine.Human.Hand.Count);
        Assert.AreEqual(GamePhase.Play, engine.Phase);
        Assert.AreEqual(MoveError.AlreadyDrew, engine.DrawFromStock());
    }

    [TestMethod]
    public void DrawnRedThreeIsHeldAndCanBeDiscarded()
    {
        Card redThree = C(Rank.Three, Suit.Hearts);
        GameEngine engine = Arrange(Many(Rank.Four, 5), new[] { C(Rank.King) });
        engine.StockList.Add(redThree);

        engine.DrawFromStock();

        Assert.AreEqual(7, engine.Human.Hand.Count);
        Assert.IsTrue(engine.Human.Hand.Contains(redThree));
        Assert.AreEqual(MoveError.None, engine.Discard(redThree));
        Assert.AreSame(redThree, engine.TopDiscard);
        Assert.AreEqual(MoveError.DiscardPileFrozen, engine.CheckPickUpDiscardPile());
    }

    [TestMethod]
    public void MeldingBeforeDrawingIsRefused()
    {
        GameEngine engine = Arrange(Many(Rank.Ace, 3), new[] { C(Rank.King) });
        Assert.AreEqual(MoveError.MustDrawFirst, engine.Meld(new List<Card>(engine.Human.Hand), null));
    }

    [TestMethod]
    public void OpeningBelowMinimumStaysPendingAndReturnsOnDiscard()
    {
        List<Card> fours = Many(Rank.Four, 3);
        Card spare = C(Rank.Nine);
        GameEngine engine = Arrange(Join(fours, new[] { spare }), new[] { C(Rank.King) }, 1, GamePhase.Play);

        Assert.AreEqual(MoveError.None, engine.Meld(fours, null));
        Assert.IsFalse(engine.Human.HasOpened);
        Assert.AreEqual(1, engine.PendingMelds.Count);
        Assert.AreEqual(15, engine.PendingPoints);

        Assert.AreEqual(MoveError.None, engine.Discard(spare));
        Assert.AreEqual(0, engine.PendingMelds.Count);
        Assert.AreEqual(3, engine.Human.Hand.Count);
        Assert.AreEqual(0, engine.Human.Melds.Count);
    }

    [TestMethod]
    public void OpeningCommitsWhenMinimumReached()
    {
        List<Card> aces = Many(Rank.Ace, 3);
        GameEngine engine = Arrange(Join(aces, new[] { C(Rank.Nine) }), new[] { C(Rank.King) }, 1, GamePhase.Play);

        Assert.AreEqual(MoveError.None, engine.Meld(aces, null));
        Assert.IsTrue(engine.Human.HasOpened);
        Assert.AreEqual(1, engine.Human.Melds.Count);
        Assert.AreEqual(0, engine.PendingMelds.Count);
    }

    [TestMethod]
    public void OpeningCanCombineSeveralMelds()
    {
        List<Card> kings = Many(Rank.King, 3);
        List<Card> queens = Many(Rank.Queen, 3);
        GameEngine engine = Arrange(Join(kings, queens, new[] { C(Rank.Four) }), new[] { C(Rank.Five) }, 1, GamePhase.Play);

        engine.Meld(kings, null);
        Assert.IsFalse(engine.Human.HasOpened);
        engine.Meld(queens, null);
        Assert.IsTrue(engine.Human.HasOpened);
        Assert.AreEqual(2, engine.Human.Melds.Count);
    }

    [TestMethod]
    public void SecondUnfinishedMeldOfSameRankIsRefused()
    {
        GameEngine engine = Arrange(Many(Rank.Ace, 7), new[] { C(Rank.Five) }, 1, GamePhase.Play);
        List<Card> hand = new(engine.Human.Hand);

        engine.Meld(hand.GetRange(0, 3), null);
        Assert.AreEqual(MoveError.DuplicateOpenMeld, engine.Meld(hand.GetRange(3, 3), null));
    }

    [TestMethod]
    public void PickupNeedsTwoMatchingNaturals()
    {
        GameEngine engine = Arrange(Join(new[] { C(Rank.King) }, Many(Rank.Four, 4)), new[] { C(Rank.King) });
        Assert.AreEqual(MoveError.PickupNeedsTwoNaturals, engine.PickUpDiscardPile());
    }

    [TestMethod]
    public void PickupRefusedWhenThreeOrWildOnTop()
    {
        GameEngine engine = Arrange(Many(Rank.King, 4), new[] { C(Rank.King), C(Rank.Three, Suit.Clubs) });
        Assert.AreEqual(MoveError.DiscardPileFrozen, engine.PickUpDiscardPile());

        engine.DiscardList.Add(C(Rank.Two));
        Assert.AreEqual(MoveError.DiscardPileFrozen, engine.PickUpDiscardPile());
    }

    [TestMethod]
    public void PickupTakesTheTopCardAndTheTwoBeneathIt()
    {
        Card second = C(Rank.Seven);
        Card third = C(Rank.Six);
        List<Card> pile = Join(Many(Rank.Four, 7), new[] { third, second, C(Rank.King) });
        GameEngine engine = Arrange(Join(Many(Rank.King, 2), Many(Rank.Five, 3)), pile);
        GiveMeld(engine.Human, Many(Rank.Ace, 3));

        Assert.AreEqual(MoveError.None, engine.PickUpDiscardPile());

        Assert.AreEqual(7, engine.DiscardPile.Count);
        Meld kings = engine.Human.FindOpenMeld(Rank.King);
        Assert.IsNotNull(kings);
        Assert.AreEqual(3, kings.Count);
        Assert.AreEqual(3 + 2, engine.Human.Hand.Count);
        Assert.IsTrue(engine.Human.Hand.Contains(second));
        Assert.IsTrue(engine.Human.Hand.Contains(third));
    }

    [TestMethod]
    public void PickupBeforeOpeningWaitsForMinimumAndCanBeTakenBack()
    {
        List<Card> pile = Join(Many(Rank.Four, 3), new[] { C(Rank.Eight) });
        Card spare = C(Rank.Nine);
        GameEngine engine = Arrange(Join(Many(Rank.Eight, 2), new[] { spare }), pile);

        Assert.AreEqual(MoveError.None, engine.PickUpDiscardPile());
        Assert.IsTrue(engine.HasPendingPickup);
        Assert.AreEqual(30, engine.PendingPoints);
        Assert.AreEqual(MoveError.OpeningMinimumNotMet, engine.Discard(spare));

        Assert.AreEqual(MoveError.None, engine.ReturnPendingMeld(engine.PendingPickupMeld));
        Assert.AreEqual(GamePhase.Draw, engine.Phase);
        Assert.AreEqual(4, engine.DiscardPile.Count);
        Assert.AreEqual(Rank.Eight, engine.TopDiscard.Rank);
        Assert.AreEqual(3, engine.Human.Hand.Count);
    }

    [TestMethod]
    public void PickupBeforeOpeningReleasesBuriedCardsOnceOpened()
    {
        List<Card> pile = Join(Many(Rank.Four, 3), new[] { C(Rank.Ace) });
        GameEngine engine = Arrange(Join(Many(Rank.Ace, 2), new[] { C(Rank.Nine) }), pile);

        Assert.AreEqual(MoveError.None, engine.PickUpDiscardPile());

        Assert.IsTrue(engine.Human.HasOpened);
        Assert.IsFalse(engine.HasPendingPickup);
        Assert.AreEqual(1, engine.DiscardPile.Count);
        Assert.AreEqual(1 + 2, engine.Human.Hand.Count);
    }

    [TestMethod]
    public void DrawingCancelsATentativePickup()
    {
        List<Card> pile = new() { C(Rank.Eight) };
        GameEngine engine = Arrange(Join(Many(Rank.Eight, 2), new[] { C(Rank.Nine) }), pile);

        engine.PickUpDiscardPile();
        Assert.AreEqual(MoveError.None, engine.DrawFromStock());

        Assert.IsFalse(engine.HasPendingPickup);
        Assert.AreEqual(1, engine.DiscardPile.Count);
        Assert.AreEqual(5, engine.Human.Hand.Count);
    }

    [TestMethod]
    public void MeldingOutTheHandPicksUpTheFoot()
    {
        List<Card> aces = Many(Rank.Ace, 3);
        GameEngine engine = Arrange(aces, new[] { C(Rank.King) }, 1, GamePhase.Play);

        Assert.AreEqual(MoveError.None, engine.Meld(aces, null));

        Assert.IsTrue(engine.Human.HasPickedUpFoot);
        Assert.AreEqual(GameRules.FOOT_SIZE, engine.Human.Hand.Count);
        Assert.AreSame(engine.Human, engine.CurrentPlayer);
    }

    [TestMethod]
    public void DiscardingTheLastHandCardPicksUpFootAndEndsTurn()
    {
        Card last = C(Rank.Nine);
        GameEngine engine = Arrange(new[] { last }, new[] { C(Rank.King) }, 1, GamePhase.Play);

        Assert.AreEqual(MoveError.None, engine.Discard(last));

        Assert.IsTrue(engine.Human.HasPickedUpFoot);
        Assert.AreEqual(GameRules.FOOT_SIZE, engine.Human.Hand.Count);
        Assert.AreSame(engine.Computer, engine.CurrentPlayer);
    }

    [TestMethod]
    public void CannotGoOutOnADiscard()
    {
        Card last = C(Rank.Nine);
        GameEngine engine = InFoot(new[] { last });
        GiveMeld(engine.Human, Many(Rank.Ace, 7));
        GiveMeld(engine.Human, Join(Many(Rank.King, 4), Many(Rank.Two, 3)));

        Assert.AreEqual(MoveError.CannotGoOutOnDiscard, engine.Discard(last));
        Assert.AreEqual(GamePhase.Play, engine.Phase);
    }

    [TestMethod]
    public void CannotGoOutWithoutRequiredBooks()
    {
        List<Card> nines = Many(Rank.Nine, 3);
        GameEngine engine = InFoot(nines);
        GiveMeld(engine.Human, Many(Rank.Ace, 7));

        Assert.AreEqual(MoveError.CannotGoOutYet, engine.Meld(nines, null));
    }

    [TestMethod]
    public void MeldLeavingOneCardInTheFootIsRefused()
    {
        List<Card> aces = Many(Rank.Ace, 3);
        GameEngine engine = InFoot(Join(aces, new[] { C(Rank.Nine) }));
        engine.Human.HasOpened = true;

        Assert.AreEqual(MoveError.CannotGoOutOnDiscard, engine.Meld(aces, null));
    }

    [TestMethod]
    public void WithTwoCardsLeftOneIsHeldAndTheOtherDiscarded()
    {
        Card nine = C(Rank.Nine);
        Card four = C(Rank.Four);
        GameEngine engine = InFoot(new[] { nine, four });
        GiveMeld(engine.Human, Many(Rank.Nine, 4));

        Assert.AreEqual(MoveError.CannotGoOutOnDiscard, engine.Meld(new[] { nine }, engine.Human.Melds[0]));
        Assert.AreEqual(MoveError.None, engine.Discard(four));
        Assert.AreEqual(1, engine.Human.Hand.Count);
        Assert.AreSame(engine.Computer, engine.CurrentPlayer);
    }

    [TestMethod]
    public void PickupLeavingOneCardInTheFootIsRefused()
    {
        GameEngine engine = InFoot(Join(Many(Rank.Queen, 2), new[] { C(Rank.Four) }));
        engine.SetTurn(engine.Human, GamePhase.Draw, 1);
        GiveMeld(engine.Human, Many(Rank.Ace, 7));
        engine.DiscardList.Clear();
        engine.DiscardList.Add(C(Rank.Queen));

        Assert.AreEqual(MoveError.CannotGoOutOnDiscard, engine.PickUpDiscardPile());
    }

    [TestMethod]
    public void UnopenedPickupInTheFootCannotGoOutOrStrandOneCard()
    {
        GameEngine emptying = InFoot(Many(Rank.Ace, 2));
        emptying.SetTurn(emptying.Human, GamePhase.Draw, 1);
        emptying.DiscardList.Clear();
        emptying.DiscardList.Add(C(Rank.Ace));

        Assert.AreEqual(MoveError.CannotGoOutYet, emptying.PickUpDiscardPile());

        GameEngine stranding = InFoot(Join(Many(Rank.Ace, 2), new[] { C(Rank.Four) }));
        stranding.SetTurn(stranding.Human, GamePhase.Draw, 1);
        stranding.DiscardList.Clear();
        stranding.DiscardList.Add(C(Rank.Ace));

        Assert.AreEqual(MoveError.CannotGoOutOnDiscard, stranding.PickUpDiscardPile());
    }

    [TestMethod]
    public void BooksInThePendingOpeningCountTowardGoingOut()
    {
        List<Card> fours = Many(Rank.Four, 7);
        List<Card> dirtyFives = Join(Many(Rank.Five, 5), Many(Rank.Two, 2));
        List<Card> aces = Many(Rank.Ace, 3);
        GameEngine engine = InFoot(Join(fours, dirtyFives, aces));
        engine.SetTurn(engine.Human, GamePhase.Play, 4);

        Assert.AreEqual(MoveError.None, engine.Meld(fours, null));
        Assert.AreEqual(MoveError.None, engine.Meld(dirtyFives, null));
        Assert.IsFalse(engine.Human.HasOpened);
        Assert.AreEqual(35 + 65, engine.PendingPoints);

        Assert.AreEqual(MoveError.None, engine.Meld(aces, null));

        Assert.AreSame(engine.Human, engine.WentOut);
        Assert.AreEqual(GamePhase.GameOver, engine.Phase);
    }

    [TestMethod]
    public void GoingOutByMeldingTheLastCardsEndsRoundAndScores()
    {
        List<Card> nines = Many(Rank.Nine, 3);
        GameEngine engine = InFoot(nines);
        GiveMeld(engine.Human, Many(Rank.Ace, 7));
        GiveMeld(engine.Human, Join(Many(Rank.King, 4), Many(Rank.Two, 3)));

        Assert.AreEqual(MoveError.None, engine.Meld(nines, null));

        Assert.AreEqual(GamePhase.RoundOver, engine.Phase);
        Assert.AreSame(engine.Human, engine.WentOut);

        RoundScore human = engine.Human.RoundScores[0];
        Assert.AreEqual(140 + 40 + 60 + 30, human.MeldPoints);
        Assert.AreEqual(GameRules.CLEAN_BOOK_BONUS + GameRules.DIRTY_BOOK_BONUS, human.BookBonus);
        Assert.AreEqual(GameRules.GOING_OUT_BONUS, human.GoingOutBonus);
        Assert.AreEqual(0, human.CardsLeftPenalty);

        RoundScore computer = engine.Computer.RoundScores[0];
        Assert.AreEqual(22 * 10, computer.CardsLeftPenalty);
        Assert.AreEqual(-220, computer.Total);
    }

    [TestMethod]
    public void RedThreeHeldAtRoundEndCostsFiveHundred()
    {
        GameEngine engine = Arrange(new[] { C(Rank.Three, Suit.Diamonds), C(Rank.Four) }, new[] { C(Rank.King) });
        engine.Human.FootList.Clear();
        engine.StockList.Clear();

        engine.DrawFromStock();

        Assert.AreEqual(500 + 5, engine.Human.RoundScores[0].CardsLeftPenalty);
    }

    [TestMethod]
    public void EmptyStockIsRebuiltFromDiscardsUnderTheTopCard()
    {
        Card top = C(Rank.King);
        GameEngine engine = Arrange(Many(Rank.Four, 3), Join(Many(Rank.Seven, 5), new[] { top }));
        engine.StockList.Clear();

        Assert.AreEqual(MoveError.None, engine.DrawFromStock());

        Assert.AreEqual(GamePhase.Play, engine.Phase);
        Assert.AreEqual(5, engine.Human.Hand.Count);
        Assert.AreEqual(3, engine.StockCount);
        Assert.AreEqual(1, engine.DiscardPile.Count);
        Assert.AreSame(top, engine.TopDiscard);
    }

    [TestMethod]
    public void StockRunningOutMidDrawIsRebuiltForTheSecondCard()
    {
        Card lastStockCard = C(Rank.Jack);
        GameEngine engine = Arrange(Many(Rank.Four, 3), Join(Many(Rank.Seven, 2), new[] { C(Rank.King) }));
        engine.StockList.Clear();
        engine.StockList.Add(lastStockCard);

        engine.DrawFromStock();

        Assert.AreEqual(5, engine.Human.Hand.Count);
        Assert.IsTrue(engine.Human.Hand.Contains(lastStockCard));
        Assert.AreEqual(1, engine.StockCount);
        Assert.AreEqual(1, engine.DiscardPile.Count);
    }

    [TestMethod]
    public void RoundEndsWhenStockAndDiscardsCannotSupplyAFullDraw()
    {
        GameEngine engine = Arrange(Many(Rank.Four, 3), Join(new[] { C(Rank.Seven) }, new[] { C(Rank.King) }));
        engine.StockList.Clear();

        Assert.AreEqual(MoveError.None, engine.DrawFromStock());

        Assert.AreEqual(GamePhase.RoundOver, engine.Phase);
        Assert.AreEqual(3, engine.Human.Hand.Count);
    }

    [TestMethod]
    public void EmptyStockEndsRoundWithoutGoingOutBonus()
    {
        GameEngine engine = Arrange(Many(Rank.Four, 3), new[] { C(Rank.King) });
        engine.StockList.Clear();

        Assert.AreEqual(MoveError.None, engine.DrawFromStock());

        Assert.AreEqual(GamePhase.RoundOver, engine.Phase);
        Assert.IsNull(engine.WentOut);
        Assert.AreEqual(0, engine.Human.RoundScores[0].GoingOutBonus);
    }

    [TestMethod]
    public void FootRedThreesArePenalisedWhenNeverPickedUp()
    {
        GameEngine engine = Arrange(new List<Card>(), new[] { C(Rank.King) });
        engine.Human.FootList.Clear();
        engine.Human.FootList.Add(C(Rank.Three, Suit.Diamonds));
        engine.Human.FootList.Add(C(Rank.Three, Suit.Spades));
        engine.StockList.Clear();

        engine.DrawFromStock();

        Assert.AreEqual(505, engine.Human.RoundScores[0].CardsLeftPenalty);
    }

    [TestMethod]
    public void OpeningMinimumRisesEachRound()
    {
        Assert.AreEqual(50, GameRules.OpeningMinimum(1));
        Assert.AreEqual(90, GameRules.OpeningMinimum(2));
        Assert.AreEqual(120, GameRules.OpeningMinimum(3));
        Assert.AreEqual(150, GameRules.OpeningMinimum(4));
    }

    /// <summary>
    /// A human turn in the play step, already playing from the foot with the given cards.
    /// </summary>
    private static GameEngine InFoot(IList<Card> hand)
    {
        GameEngine engine = Arrange(hand, new[] { C(Rank.King) }, 1, GamePhase.Play);
        engine.Human.HasPickedUpFoot = true;
        engine.Human.FootList.Clear();
        return engine;
    }
}
