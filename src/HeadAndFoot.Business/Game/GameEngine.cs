using HeadAndFoot.Business.Cards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeadAndFoot.Business.Game;

/// <summary>
/// Runs a two-player game of Head and Foot: dealing, turn order, move validation,
/// opening requirements, the foot, going out, and scoring.
/// </summary>
/// <remarks>
/// Before a player has opened in a round, the melds they lay down are held as
/// <see cref="PendingMelds"/> until their printed value reaches the round's minimum,
/// at which point they are committed together. This lets an opening be built from
/// several melds one at a time, as at a real table.
/// </remarks>
public sealed class GameEngine
{
    private readonly Random _random;
    private readonly Player[] _players;
    private readonly List<Card> _stock = new();
    private readonly List<Card> _discardPile = new();
    private readonly List<Meld> _pendingMelds = new();
    private readonly List<Card> _pendingPickupBuried = new();
    private Meld _pendingPickupMeld;
    private Card _pendingPickupTopCard;
    private int _currentPlayerIndex;

    public GameEngine(Random random)
    {
        if (random == null)
        {
            throw new ArgumentNullException(nameof(random));
        }

        _random = random;
        _players = new Player[] { new Player("You", false), new Player("Computer", true) };
        Players = new ReadOnlyCollection<Player>(_players);
        DiscardPile = _discardPile.AsReadOnly();
        PendingMelds = _pendingMelds.AsReadOnly();
        LastEvent = string.Empty;
    }

    public ReadOnlyCollection<Player> Players { get; }

    public Player Human => _players[0];

    public Player Computer => _players[1];

    public int Round { get; private set; }

    public int OpeningMinimum => GameRules.OpeningMinimum(Round);

    public GamePhase Phase { get; private set; }

    public bool IsRoundInProgress => Phase == GamePhase.Draw || Phase == GamePhase.Play;

    public Player CurrentPlayer => _players[_currentPlayerIndex];

    public int StockCount => _stock.Count;

    /// <summary>
    /// The discard pile, bottom first. The last card is the top card.
    /// </summary>
    public ReadOnlyCollection<Card> DiscardPile { get; }

    public Card TopDiscard => _discardPile.Count > 0 ? _discardPile[_discardPile.Count - 1] : null;

    /// <summary>
    /// Melds laid down by the current player toward an opening that has not yet reached the minimum.
    /// </summary>
    public ReadOnlyCollection<Meld> PendingMelds { get; }

    public bool HasPendingPickup => _pendingPickupMeld != null;

    /// <summary>
    /// The discard pile meld that must be committed (or taken back) before the turn can end.
    /// </summary>
    public Meld PendingPickupMeld => _pendingPickupMeld;

    public int PendingPoints
    {
        get
        {
            int points = 0;
            foreach (Meld meld in _pendingMelds)
            {
                points += meld.CardPoints;
            }

            return points;
        }
    }

    /// <summary>
    /// A short description of the most recent thing that happened, for display.
    /// </summary>
    public string LastEvent { get; private set; }

    /// <summary>
    /// The player who went out to end the last round, or null if the round ended because the stock
    /// ran out with too few discards left to reshuffle into it.
    /// </summary>
    public Player WentOut { get; private set; }

    public Player Opponent(Player player)
    {
        return player == _players[0] ? _players[1] : _players[0];
    }

    /// <summary>
    /// The leading player once the game is over, or null when the totals are tied.
    /// </summary>
    public Player Winner
    {
        get
        {
            if (Human.TotalScore == Computer.TotalScore)
            {
                return null;
            }

            return Human.TotalScore > Computer.TotalScore ? Human : Computer;
        }
    }

    public void StartNewGame()
    {
        foreach (Player player in _players)
        {
            player.RoundScoreList.Clear();
        }

        StartRound(1);
    }

    public void StartNextRound()
    {
        if (Phase != GamePhase.RoundOver)
        {
            throw new InvalidOperationException("The next round can only start after a round ends.");
        }

        StartRound(Round + 1);
    }

    public MoveError CheckPickUpDiscardPile()
    {
        if (!IsRoundInProgress)
        {
            return MoveError.GameNotInProgress;
        }

        if (Phase != GamePhase.Draw)
        {
            return MoveError.AlreadyDrew;
        }

        Card top = TopDiscard;
        if (top == null)
        {
            return MoveError.DiscardPileEmpty;
        }

        if (!top.IsNatural)
        {
            return MoveError.DiscardPileFrozen;
        }

        Player player = CurrentPlayer;
        if (CountNaturals(player.HandList, top.Rank) < GameRules.NATURALS_NEEDED_FOR_PICKUP)
        {
            return MoveError.PickupNeedsTwoNaturals;
        }

        if (player.HasPickedUpFoot && player.HasOpened)
        {
            // Mirror PickUpDiscardPile: the top card and two naturals join the open meld of
            // that rank (or start one), and any that do not fit once it is a book stay in hand.
            Meld target = player.FindOpenMeld(top.Rank);
            int countBefore = target == null ? 0 : target.Count;
            int meldCards = GameRules.NATURALS_NEEDED_FOR_PICKUP + 1;
            int added = Math.Min(meldCards, GameRules.BOOK_SIZE - countBefore);
            int buried = Math.Min(_discardPile.Count - 1, GameRules.MAXIMUM_PICKUP - 1);
            int remaining = player.HandList.Count - GameRules.NATURALS_NEEDED_FOR_PICKUP + buried + (meldCards - added);

            int clean = player.CleanBookCount;
            int dirty = player.DirtyBookCount;
            if (countBefore + added >= GameRules.BOOK_SIZE)
            {
                if (target.WildCount == 0)
                {
                    clean++;
                }
                else
                {
                    dirty++;
                }
            }

            MoveError handError = CheckCardsLeftOnFoot(remaining, clean >= 1 && dirty >= 1);
            if (handError != MoveError.None)
            {
                return handError;
            }
        }
        else if (player.HasPickedUpFoot)
        {
            // Not yet opened: when the pickup meld alone reaches the minimum, the opening commits
            // at once and the buried cards join the hand, so the foot's rules apply right away.
            // Otherwise the pickup stays tentative, and drawing from the stock can still undo it.
            int pickupPoints = top.PointValue * (GameRules.NATURALS_NEEDED_FOR_PICKUP + 1);
            if (pickupPoints >= OpeningMinimum)
            {
                int buried = Math.Min(_discardPile.Count - 1, GameRules.MAXIMUM_PICKUP - 1);
                int remaining = player.HandList.Count - GameRules.NATURALS_NEEDED_FOR_PICKUP + buried;
                MoveError handError = CheckCardsLeftOnFoot(remaining, player.HasRequiredBooks);
                if (handError != MoveError.None)
                {
                    return handError;
                }
            }
        }

        return MoveError.None;
    }

    public MoveError DrawFromStock()
    {
        if (!IsRoundInProgress)
        {
            return MoveError.GameNotInProgress;
        }

        if (Phase == GamePhase.Play && _pendingPickupMeld != null)
        {
            CancelPendingPickup();
        }

        if (Phase != GamePhase.Draw)
        {
            return MoveError.AlreadyDrew;
        }

        Player player = CurrentPlayer;

        // Every draw takes a full two cards, reshuffling the discards under the top card when
        // the stock runs short. Because each turn then draws two and discards one, the cards in
        // circulation always shrink, so a round that nobody can go out of still comes to an end.
        int reshufflable = Math.Max(0, _discardPile.Count - 1);
        if (_stock.Count + reshufflable < GameRules.CARDS_DRAWN_FROM_STOCK)
        {
            LastEvent = "The stock has run out, with too few discards left to reshuffle.";
            EndRound(null);
            return MoveError.None;
        }

        bool reshuffled = false;
        List<Card> drawn = new();
        while (drawn.Count < GameRules.CARDS_DRAWN_FROM_STOCK)
        {
            if (_stock.Count == 0)
            {
                ReshuffleDiscardsIntoStock();
                reshuffled = true;
            }

            drawn.Add(PopStock());
        }

        player.HandList.AddRange(drawn);
        Phase = GamePhase.Play;

        LastEvent = (reshuffled ? "The discards were shuffled into a new stock. " : string.Empty)
            + player.Name + " drew two cards from the stock.";
        return MoveError.None;
    }

    public MoveError PickUpDiscardPile()
    {
        MoveError error = CheckPickUpDiscardPile();
        if (error != MoveError.None)
        {
            return error;
        }

        Player player = CurrentPlayer;
        Card top = _discardPile[_discardPile.Count - 1];
        _discardPile.RemoveAt(_discardPile.Count - 1);

        List<Card> meldCards = new() { top };
        for (int i = player.HandList.Count - 1; i >= 0 && meldCards.Count <= GameRules.NATURALS_NEEDED_FOR_PICKUP; i--)
        {
            Card card = player.HandList[i];
            if (card.IsNatural && card.Rank == top.Rank)
            {
                meldCards.Add(card);
                player.HandList.RemoveAt(i);
            }
        }

        List<Card> buried = new();
        while (buried.Count < GameRules.MAXIMUM_PICKUP - 1 && _discardPile.Count > 0)
        {
            buried.Add(_discardPile[_discardPile.Count - 1]);
            _discardPile.RemoveAt(_discardPile.Count - 1);
        }

        Phase = GamePhase.Play;
        string pileText = buried.Count == 0 ? "the top discard" : "the discard pile (" + (buried.Count + 1) + " cards)";

        if (player.HasOpened)
        {
            Meld target = player.FindOpenMeld(top.Rank);
            if (target == null)
            {
                target = new Meld(top.Rank);
                player.MeldList.Add(target);
            }

            foreach (Card card in meldCards)
            {
                if (target.IsBook)
                {
                    player.HandList.Add(card);
                }
                else
                {
                    target.CardList.Add(card);
                }
            }

            OrderMeld(target);
            player.HandList.AddRange(buried);
            LastEvent = player.Name + " picked up " + pileText + " and melded " + Card.RankPluralName(top.Rank) + ".";
            HandleEmptyHandAfterMeld(player);
            return MoveError.None;
        }

        Meld pending = new(top.Rank);
        pending.CardList.AddRange(meldCards);
        _pendingMelds.Add(pending);
        _pendingPickupMeld = pending;
        _pendingPickupTopCard = top;
        _pendingPickupBuried.Clear();
        _pendingPickupBuried.AddRange(buried);

        LastEvent = player.Name + " picked up " + pileText + " toward opening with " + Card.RankPluralName(top.Rank) + ".";
        TryCommitOpening(player);
        return MoveError.None;
    }

    /// <summary>
    /// Checks whether <paramref name="cards"/> from the current player's hand can start a new meld
    /// (when <paramref name="target"/> is null) or be added to <paramref name="target"/>.
    /// </summary>
    public MoveError CheckMeld(IList<Card> cards, Meld target)
    {
        if (!IsRoundInProgress)
        {
            return MoveError.GameNotInProgress;
        }

        if (Phase != GamePhase.Play)
        {
            return MoveError.MustDrawFirst;
        }

        if (cards == null || cards.Count == 0)
        {
            return MoveError.NoCardsSelected;
        }

        Player player = CurrentPlayer;
        if (!AllInHand(player, cards))
        {
            return MoveError.CardNotInHand;
        }

        MoveError error;
        int naturalsAfter;
        int wildsAfter;

        if (target != null)
        {
            if (!_pendingMelds.Contains(target) && !player.MeldList.Contains(target))
            {
                return MoveError.NotYourMeld;
            }

            error = MeldRules.ValidateAddition(target, cards);
            naturalsAfter = target.NaturalCount;
            wildsAfter = target.WildCount;
        }
        else
        {
            error = MeldRules.ValidateNew(cards, out Rank rank);
            if (error == MoveError.None && HasUnfinishedMeld(player, rank))
            {
                error = MoveError.DuplicateOpenMeld;
            }

            naturalsAfter = 0;
            wildsAfter = 0;
        }

        if (error != MoveError.None)
        {
            return error;
        }

        int addedPoints = 0;
        foreach (Card card in cards)
        {
            addedPoints += card.PointValue;
            if (card.IsWild)
            {
                wildsAfter++;
            }
            else
            {
                naturalsAfter++;
            }
        }

        int remaining = player.HandList.Count - cards.Count;
        bool openedAfter = player.HasOpened || PendingPoints + addedPoints >= OpeningMinimum;

        if (!openedAfter && remaining == 0)
        {
            return MoveError.MustKeepCardToDiscard;
        }

        if (!player.HasOpened && openedAfter)
        {
            // Completing the opening releases any discard pile cards held aside into the hand.
            remaining += _pendingPickupBuried.Count;
        }

        if (player.HasPickedUpFoot)
        {
            int clean = player.CleanBookCount;
            int dirty = player.DirtyBookCount;
            if (naturalsAfter + wildsAfter >= GameRules.BOOK_SIZE)
            {
                if (wildsAfter == 0)
                {
                    clean++;
                }
                else
                {
                    dirty++;
                }
            }

            if (!player.HasOpened && openedAfter)
            {
                // Completing the opening commits every pending meld, so their books count too.
                // The meld being changed was counted above with this move's cards included.
                foreach (Meld pending in _pendingMelds)
                {
                    if (pending == target || !pending.IsBook)
                    {
                        continue;
                    }

                    if (pending.IsClean)
                    {
                        clean++;
                    }
                    else
                    {
                        dirty++;
                    }
                }
            }

            MoveError handError = CheckCardsLeftOnFoot(remaining, clean >= 1 && dirty >= 1);
            if (handError != MoveError.None)
            {
                return handError;
            }
        }

        return MoveError.None;
    }

    public MoveError Meld(IList<Card> cards, Meld target)
    {
        MoveError error = CheckMeld(cards, target);
        if (error != MoveError.None)
        {
            return error;
        }

        Player player = CurrentPlayer;
        List<Card> moving = new(cards);
        foreach (Card card in moving)
        {
            player.HandList.Remove(card);
        }

        if (target == null)
        {
            Rank rank = Rank.Joker;
            foreach (Card card in moving)
            {
                if (card.IsNatural)
                {
                    rank = card.Rank;
                    break;
                }
            }

            target = new Meld(rank);
            if (player.HasOpened)
            {
                player.MeldList.Add(target);
            }
            else
            {
                _pendingMelds.Add(target);
            }
        }

        bool isNewMeld = target.Count == 0;
        target.CardList.AddRange(moving);
        OrderMeld(target);

        LastEvent = isNewMeld
            ? player.Name + " melded " + moving.Count + " " + Card.RankPluralName(target.Rank) + "."
            : player.Name + " added " + (moving.Count == 1 ? "a card" : moving.Count + " cards") + " to the " + Card.RankPluralName(target.Rank) + ".";
        if (target.IsBook && player.HasOpened)
        {
            LastEvent = player.Name + " completed a " + (target.IsClean ? "clean" : "dirty") + " book of " + Card.RankPluralName(target.Rank) + "!";
        }

        if (player.HasOpened)
        {
            HandleEmptyHandAfterMeld(player);
        }
        else
        {
            TryCommitOpening(player);
        }

        return MoveError.None;
    }

    /// <summary>
    /// Takes back a meld laid down toward an opening that has not been committed yet.
    /// Taking back the discard pile meld returns the whole pile and the turn to its draw step.
    /// </summary>
    public MoveError ReturnPendingMeld(Meld meld)
    {
        if (!IsRoundInProgress)
        {
            return MoveError.GameNotInProgress;
        }

        if (Phase != GamePhase.Play || meld == null || !_pendingMelds.Contains(meld))
        {
            return MoveError.NothingToReturn;
        }

        Player player = CurrentPlayer;
        if (meld == _pendingPickupMeld)
        {
            CancelPendingPickup();
            LastEvent = player.Name + " put the discard pile back.";
            return MoveError.None;
        }

        _pendingMelds.Remove(meld);
        player.HandList.AddRange(meld.CardList);
        LastEvent = player.Name + " took back the " + Card.RankPluralName(meld.Rank) + ".";
        return MoveError.None;
    }

    public MoveError CheckDiscard(Card card)
    {
        if (!IsRoundInProgress)
        {
            return MoveError.GameNotInProgress;
        }

        if (Phase != GamePhase.Play)
        {
            return MoveError.MustDrawFirst;
        }

        if (_pendingPickupMeld != null)
        {
            return MoveError.OpeningMinimumNotMet;
        }

        Player player = CurrentPlayer;
        if (card == null || !player.HandList.Contains(card))
        {
            return MoveError.CardNotInHand;
        }

        int remaining = player.HandList.Count + CountPendingCards() - 1;
        if (remaining == 0 && player.HasPickedUpFoot)
        {
            return MoveError.CannotGoOutOnDiscard;
        }

        return MoveError.None;
    }

    public MoveError Discard(Card card)
    {
        MoveError error = CheckDiscard(card);
        if (error != MoveError.None)
        {
            return error;
        }

        Player player = CurrentPlayer;
        ReturnAllPendingMelds(player);

        player.HandList.Remove(card);
        _discardPile.Add(card);
        LastEvent = player.Name + " discarded the " + card + ".";

        if (player.HandList.Count == 0)
        {
            // CheckDiscard only allows the last card to go when the foot is still waiting.
            PickUpFoot(player);
            LastEvent += " " + player.Name + " picked up " + Possessive(player) + " foot.";
        }

        _currentPlayerIndex = 1 - _currentPlayerIndex;
        Phase = GamePhase.Draw;
        return MoveError.None;
    }

    internal List<Card> StockList => _stock;

    internal List<Card> DiscardList => _discardPile;

    internal List<Card> PendingPickupBuriedList => _pendingPickupBuried;

    /// <summary>
    /// Places the game at the start of a turn. Used to arrange specific situations in tests.
    /// </summary>
    internal void SetTurn(Player player, GamePhase phase, int round)
    {
        _currentPlayerIndex = player == _players[0] ? 0 : 1;
        Phase = phase;
        Round = round;
    }

    private void StartRound(int round)
    {
        Round = round;
        WentOut = null;
        _pendingMelds.Clear();
        _pendingPickupMeld = null;
        _pendingPickupTopCard = null;
        _pendingPickupBuried.Clear();
        _stock.Clear();
        _discardPile.Clear();

        foreach (Player player in _players)
        {
            player.ResetForRound();
        }

        _stock.AddRange(Shoe.Create(GameRules.DECK_COUNT));
        Shoe.Shuffle(_stock, _random);

        List<Card>[] dealtHands = new List<Card>[] { new(), new() };
        for (int i = 0; i < GameRules.HAND_SIZE; i++)
        {
            dealtHands[0].Add(PopStock());
            dealtHands[1].Add(PopStock());
        }

        for (int i = 0; i < GameRules.FOOT_SIZE; i++)
        {
            _players[0].FootList.Add(PopStock());
            _players[1].FootList.Add(PopStock());
        }

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i].HandList.AddRange(dealtHands[i]);
        }

        Card upcard = PopStock();
        _discardPile.Add(upcard);
        while ((upcard.IsThree || upcard.IsWild) && _stock.Count > 0)
        {
            upcard = PopStock();
            _discardPile.Add(upcard);
        }

        _currentPlayerIndex = (round - 1) % GameRules.PLAYER_COUNT;
        Phase = GamePhase.Draw;
        LastEvent = "Round " + round + " dealt. Opening minimum is " + OpeningMinimum + " points.";
    }

    /// <summary>
    /// Turns every discard except the top card into a freshly shuffled stock.
    /// </summary>
    private void ReshuffleDiscardsIntoStock()
    {
        Card top = _discardPile[_discardPile.Count - 1];
        _discardPile.RemoveAt(_discardPile.Count - 1);
        _stock.AddRange(_discardPile);
        _discardPile.Clear();
        _discardPile.Add(top);
        Shoe.Shuffle(_stock, _random);
    }

    private Card PopStock()
    {
        Card card = _stock[_stock.Count - 1];
        _stock.RemoveAt(_stock.Count - 1);
        return card;
    }

    private static void PickUpFoot(Player player)
    {
        player.HandList.AddRange(player.FootList);
        player.FootList.Clear();
        player.HasPickedUpFoot = true;
    }

    /// <summary>
    /// After a meld empties the hand: a player still holding a foot picks it up and plays on,
    /// and a player already in their foot has gone out (the move checks ensured they may).
    /// </summary>
    private void HandleEmptyHandAfterMeld(Player player)
    {
        if (player.HandList.Count != 0)
        {
            return;
        }

        if (player.HasPickedUpFoot)
        {
            LastEvent = player.Name + (player.IsComputer ? " went out by melding its last card!" : " went out by melding your last card!");
            EndRound(player);
            return;
        }

        PickUpFoot(player);
        LastEvent += " " + player.Name + " played out " + Possessive(player) + " hand and picked up " + Possessive(player) + " foot.";
    }

    /// <summary>
    /// Once the foot is in play a player must either meld every card to go out, or keep at
    /// least two: one to discard and one to hold, since going out on a discard is not allowed.
    /// </summary>
    private static MoveError CheckCardsLeftOnFoot(int remaining, bool canGoOut)
    {
        if (remaining == 0)
        {
            return canGoOut ? MoveError.None : MoveError.CannotGoOutYet;
        }

        return remaining == 1 ? MoveError.CannotGoOutOnDiscard : MoveError.None;
    }

    private void TryCommitOpening(Player player)
    {
        if (player.HasOpened || _pendingMelds.Count == 0 || PendingPoints < OpeningMinimum)
        {
            return;
        }

        int points = PendingPoints;
        player.MeldList.AddRange(_pendingMelds);
        _pendingMelds.Clear();
        player.HasOpened = true;
        LastEvent = player.Name + " opened with " + points + " points.";

        if (_pendingPickupMeld != null)
        {
            List<Card> buried = new(_pendingPickupBuried);
            _pendingPickupMeld = null;
            _pendingPickupTopCard = null;
            _pendingPickupBuried.Clear();
            player.HandList.AddRange(buried);
        }

        HandleEmptyHandAfterMeld(player);
    }

    private void CancelPendingPickup()
    {
        Player player = CurrentPlayer;
        Card top = _pendingPickupTopCard;

        foreach (Meld meld in _pendingMelds)
        {
            foreach (Card card in meld.CardList)
            {
                if (card != top)
                {
                    player.HandList.Add(card);
                }
            }
        }

        _pendingMelds.Clear();

        for (int i = _pendingPickupBuried.Count - 1; i >= 0; i--)
        {
            _discardPile.Add(_pendingPickupBuried[i]);
        }

        _discardPile.Add(top);
        _pendingPickupBuried.Clear();
        _pendingPickupMeld = null;
        _pendingPickupTopCard = null;
        Phase = GamePhase.Draw;
    }

    private void ReturnAllPendingMelds(Player player)
    {
        foreach (Meld meld in _pendingMelds)
        {
            player.HandList.AddRange(meld.CardList);
        }

        _pendingMelds.Clear();
    }

    private int CountPendingCards()
    {
        int count = 0;
        foreach (Meld meld in _pendingMelds)
        {
            count += meld.Count;
        }

        return count;
    }

    private void EndRound(Player wentOut)
    {
        WentOut = wentOut;
        ReturnAllPendingMelds(CurrentPlayer);
        foreach (Player player in _players)
        {
            player.RoundScoreList.Add(ScoreRound(player, player == wentOut));
        }

        Phase = Round >= GameRules.ROUND_COUNT ? GamePhase.GameOver : GamePhase.RoundOver;
    }

    private RoundScore ScoreRound(Player player, bool wentOut)
    {
        int meldPoints = 0;
        int bookBonus = 0;
        foreach (Meld meld in player.MeldList)
        {
            meldPoints += meld.CardPoints;
            bookBonus += meld.BookBonus;
        }

        int penalty = 0;
        foreach (Card card in player.HandList)
        {
            penalty += card.PointValue;
        }

        foreach (Card card in player.FootList)
        {
            penalty += card.PointValue;
        }

        return new RoundScore(
            Round,
            meldPoints,
            bookBonus,
            wentOut ? GameRules.GOING_OUT_BONUS : 0,
            penalty);
    }

    private bool HasUnfinishedMeld(Player player, Rank rank)
    {
        if (player.FindOpenMeld(rank) != null)
        {
            return true;
        }

        foreach (Meld meld in _pendingMelds)
        {
            if (meld.Rank == rank && !meld.IsBook)
            {
                return true;
            }
        }

        return false;
    }

    private static bool AllInHand(Player player, IList<Card> cards)
    {
        List<Card> seen = new();
        foreach (Card card in cards)
        {
            if (card == null || seen.Contains(card) || !player.HandList.Contains(card))
            {
                return false;
            }

            seen.Add(card);
        }

        return true;
    }

    private static int CountNaturals(List<Card> cards, Rank rank)
    {
        int count = 0;
        foreach (Card card in cards)
        {
            if (card.IsNatural && card.Rank == rank)
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    /// Keeps natural cards ahead of wild cards so melds display consistently.
    /// </summary>
    private static void OrderMeld(Meld meld)
    {
        List<Card> naturals = new();
        List<Card> wilds = new();
        foreach (Card card in meld.CardList)
        {
            if (card.IsWild)
            {
                wilds.Add(card);
            }
            else
            {
                naturals.Add(card);
            }
        }

        meld.CardList.Clear();
        meld.CardList.AddRange(naturals);
        meld.CardList.AddRange(wilds);
    }

    private static string Possessive(Player player)
    {
        return player.IsComputer ? "its" : "your";
    }
}
