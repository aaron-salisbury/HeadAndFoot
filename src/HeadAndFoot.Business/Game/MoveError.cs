namespace HeadAndFoot.Business.Game;

/// <summary>
/// The anticipated outcomes of a player move. <see cref="None"/> means the move succeeded.
/// </summary>
public enum MoveError
{
    None = 0,
    GameNotInProgress,
    NotYourTurn,
    MustDrawFirst,
    AlreadyDrew,
    DiscardPileEmpty,
    DiscardPileFrozen,
    PickupNeedsTwoNaturals,
    CardNotInHand,
    NoCardsSelected,
    MeldTooSmall,
    MixedRanks,
    ThreesCannotBeMelded,
    NoNaturalCards,
    TooManyWilds,
    MeldFull,
    DuplicateOpenMeld,
    NotYourMeld,
    MustKeepCardToDiscard,
    CannotGoOutYet,
    CannotGoOutOnDiscard,
    OpeningMinimumNotMet,
    NothingToReturn
}

public static class MoveErrors
{
    public static string Describe(MoveError error)
    {
        switch (error)
        {
            case MoveError.None:
                return string.Empty;
            case MoveError.GameNotInProgress:
                return "The round is over.";
            case MoveError.NotYourTurn:
                return "It is not your turn.";
            case MoveError.MustDrawFirst:
                return "Draw from the stock or pick up the discard pile first.";
            case MoveError.AlreadyDrew:
                return "You have already drawn this turn.";
            case MoveError.DiscardPileEmpty:
                return "The discard pile is empty.";
            case MoveError.DiscardPileFrozen:
                return "The discard pile cannot be picked up while a three or a wild card is on top.";
            case MoveError.PickupNeedsTwoNaturals:
                return "To pick up the discard pile you need two natural cards matching the top card.";
            case MoveError.CardNotInHand:
                return "That card is not in your hand.";
            case MoveError.NoCardsSelected:
                return "Select one or more cards first.";
            case MoveError.MeldTooSmall:
                return "A new meld needs at least three cards.";
            case MoveError.MixedRanks:
                return "A meld may only contain one rank, plus wild cards.";
            case MoveError.ThreesCannotBeMelded:
                return "Threes cannot be melded.";
            case MoveError.NoNaturalCards:
                return "A meld needs natural cards, not only wild cards.";
            case MoveError.TooManyWilds:
                return "Natural cards must outnumber wild cards in a meld.";
            case MoveError.MeldFull:
                return "A book is complete at seven cards.";
            case MoveError.DuplicateOpenMeld:
                return "You already have an unfinished meld of that rank.";
            case MoveError.NotYourMeld:
                return "You can only add to your own melds.";
            case MoveError.MustKeepCardToDiscard:
                return "You must keep a card to discard.";
            case MoveError.CannotGoOutYet:
                return "You need a clean book and a dirty book, and must be playing from your foot, to go out.";
            case MoveError.CannotGoOutOnDiscard:
                return "You cannot go out on a discard. Keep a card to hold and one to discard, or meld every card to go out.";
            case MoveError.OpeningMinimumNotMet:
                return "Your opening melds do not reach this round's minimum yet.";
            case MoveError.NothingToReturn:
                return "Only melds laid down toward this turn's opening can be taken back.";
            default:
                return error.ToString();
        }
    }
}
