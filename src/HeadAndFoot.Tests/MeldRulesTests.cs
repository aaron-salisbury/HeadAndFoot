using HeadAndFoot.Business.Cards;
using HeadAndFoot.Business.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using static HeadAndFoot.Tests.TestCards;

namespace HeadAndFoot.Tests;

[TestClass]
public class MeldRulesTests
{
    [TestMethod]
    public void ThreeNaturalsFormAMeld()
    {
        Assert.AreEqual(MoveError.None, MeldRules.ValidateNew(Many(Rank.King, 3), out Rank rank));
        Assert.AreEqual(Rank.King, rank);
    }

    [TestMethod]
    public void TwoNaturalsAndOneWildFormAMeld()
    {
        List<Card> cards = Join(Many(Rank.Eight, 2), new[] { C(Rank.Two) });
        Assert.AreEqual(MoveError.None, MeldRules.ValidateNew(cards, out _));
    }

    [TestMethod]
    public void WildsMayNotEqualNaturals()
    {
        List<Card> cards = Join(Many(Rank.Eight, 2), new[] { C(Rank.Two), C(Rank.Joker) });
        Assert.AreEqual(MoveError.TooManyWilds, MeldRules.ValidateNew(cards, out _));
    }

    [TestMethod]
    public void TwoCardsAreTooFew()
    {
        Assert.AreEqual(MoveError.MeldTooSmall, MeldRules.ValidateNew(Many(Rank.Ace, 2), out _));
    }

    [TestMethod]
    public void RanksMayNotBeMixed()
    {
        List<Card> cards = Join(Many(Rank.Eight, 2), new[] { C(Rank.Nine) });
        Assert.AreEqual(MoveError.MixedRanks, MeldRules.ValidateNew(cards, out _));
    }

    [TestMethod]
    public void ThreesCannotBeMelded()
    {
        Assert.AreEqual(MoveError.ThreesCannotBeMelded, MeldRules.ValidateNew(Many(Rank.Three, 3), out _));
    }

    [TestMethod]
    public void WildsAloneCannotMeld()
    {
        List<Card> cards = Join(Many(Rank.Two, 2), new[] { C(Rank.Joker) });
        Assert.AreEqual(MoveError.NoNaturalCards, MeldRules.ValidateNew(cards, out _));
    }

    [TestMethod]
    public void MeldCannotExceedABook()
    {
        Assert.AreEqual(MoveError.MeldFull, MeldRules.ValidateNew(Many(Rank.Queen, 8), out _));
    }

    [TestMethod]
    public void AdditionsCountExistingCards()
    {
        GameEngine engine = Arrange(new List<Card>(), new List<Card>());
        Meld meld = GiveMeld(engine.Human, Join(Many(Rank.Seven, 3), new[] { C(Rank.Two) }));

        Assert.AreEqual(MoveError.None, MeldRules.ValidateAddition(meld, new[] { C(Rank.Joker), C(Rank.Seven) }));
        Assert.AreEqual(MoveError.TooManyWilds, MeldRules.ValidateAddition(meld, new[] { C(Rank.Joker), C(Rank.Two) }));
        Assert.AreEqual(MoveError.MixedRanks, MeldRules.ValidateAddition(meld, new[] { C(Rank.Six) }));
    }

    [TestMethod]
    public void CompletedBookIsClosed()
    {
        GameEngine engine = Arrange(new List<Card>(), new List<Card>());
        Meld meld = GiveMeld(engine.Human, Many(Rank.Seven, 7));

        Assert.IsTrue(meld.IsBook);
        Assert.IsTrue(meld.IsClean);
        Assert.AreEqual(GameRules.CLEAN_BOOK_BONUS, meld.BookBonus);
        Assert.AreEqual(MoveError.MeldFull, MeldRules.ValidateAddition(meld, new[] { C(Rank.Seven) }));
    }

    [TestMethod]
    public void DirtyBookEarnsDirtyBonus()
    {
        GameEngine engine = Arrange(new List<Card>(), new List<Card>());
        Meld meld = GiveMeld(engine.Human, Join(Many(Rank.Seven, 4), Many(Rank.Two, 3)));

        Assert.IsTrue(meld.IsBook);
        Assert.IsFalse(meld.IsClean);
        Assert.AreEqual(GameRules.DIRTY_BOOK_BONUS, meld.BookBonus);
    }
}
