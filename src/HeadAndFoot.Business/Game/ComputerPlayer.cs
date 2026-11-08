using HeadAndFoot.Business.Cards;
using System;
using System.Collections.Generic;

namespace HeadAndFoot.Business.Game;

/// <summary>
/// A straightforward computer opponent. It plays one action at a time so the
/// table can show each step, and it only acts through the engine's public moves,
/// so every action it takes is validated by the same rules as the human's.
/// </summary>
public sealed class ComputerPlayer
{
    private readonly GameEngine _engine;
    private readonly Player _player;

    public ComputerPlayer(GameEngine engine, Player player)
    {
        if (engine == null)
        {
            throw new ArgumentNullException(nameof(engine));
        }

        if (player == null)
        {
            throw new ArgumentNullException(nameof(player));
        }

        _engine = engine;
        _player = player;
    }

    private Player Me => _player;

    /// <summary>
    /// True when the engine is waiting on the computer.
    /// </summary>
    public bool IsTurn => _engine.IsRoundInProgress && _engine.CurrentPlayer == Me;

    /// <summary>
    /// Performs the computer's next single action: a draw, one meld, or the closing discard.
    /// </summary>
    public void TakeNextAction()
    {
        if (!IsTurn)
        {
            return;
        }

        if (_engine.Phase == GamePhase.Draw)
        {
            Draw();
            return;
        }

        if (_engine.HasPendingPickup && !PlanReachesOpening())
        {
            // The opening fell through; put the pile back and draw instead.
            _engine.DrawFromStock();
            return;
        }

        if (TryMeld())
        {
            return;
        }

        Discard();
    }

    private void Draw()
    {
        if (_engine.CheckPickUpDiscardPile() == MoveError.None)
        {
            _engine.PickUpDiscardPile();
            if (_engine.HasPendingPickup && !PlanReachesOpening())
            {
                // Drawing from the stock takes the tentative pickup back first.
                _engine.DrawFromStock();
            }

            return;
        }

        _engine.DrawFromStock();
    }

    private bool PlanReachesOpening()
    {
        List<PlannedMeld> plan = BuildOpeningPlan();
        return plan.Count > 0 || _engine.PendingPoints >= _engine.OpeningMinimum;
    }

    private bool TryMeld()
    {
        List<PlannedMeld> plan = Me.HasOpened ? BuildPlayPlan() : BuildOpeningPlan();

        foreach (PlannedMeld planned in plan)
        {
            List<Card> cards = new(planned.Cards);
            while (cards.Count > 0)
            {
                MoveError error = _engine.Meld(cards, planned.Target);
                if (error == MoveError.None)
                {
                    return true;
                }

                // When the meld would leave too few cards, try adding one card fewer to an existing meld.
                bool canShrink = planned.Target != null && (error == MoveError.MustKeepCardToDiscard || error == MoveError.CannotGoOutYet || error == MoveError.CannotGoOutOnDiscard);
                if (!canShrink)
                {
                    break;
                }

                cards.RemoveAt(cards.Count - 1);
            }
        }

        return false;
    }

    private void Discard()
    {
        List<Card> hand = new(Me.Hand);
        hand.Sort(delegate(Card a, Card b)
        {
            int compare = DiscardKeepScore(a).CompareTo(DiscardKeepScore(b));
            return compare != 0 ? compare : a.Id.CompareTo(b.Id);
        });

        foreach (Card card in hand)
        {
            if (_engine.Discard(card) == MoveError.None)
            {
                return;
            }
        }

        // Every discard was refused, which only happens while a pickup is pending.
        _engine.DrawFromStock();
    }

    /// <summary>
    /// Higher scores mean the card is more worth keeping.
    /// </summary>
    private int DiscardKeepScore(Card card)
    {
        if (card.IsRedThree)
        {
            // Red threes cannot be melded and cost 500 if still held when the round ends.
            return -200;
        }

        if (card.IsBlackThree)
        {
            // Black threes cannot be melded, and one on top blocks the pile.
            return -100;
        }

        if (card.IsWild)
        {
            return 1000 + card.PointValue;
        }

        int score = card.PointValue;
        int sameRank = 0;
        foreach (Card other in Me.Hand)
        {
            if (other.IsNatural && other.Rank == card.Rank)
            {
                sameRank++;
            }
        }

        score += 40 * (sameRank - 1);

        if (Me.FindOpenMeld(card.Rank) != null)
        {
            score += 60;
        }

        if (_engine.Opponent(Me).FindOpenMeld(card.Rank) != null)
        {
            // Avoid feeding the opponent ranks they are already collecting.
            score += 15;
        }

        return score;
    }

    /// <summary>
    /// Builds melds that together reach the opening minimum, or returns nothing.
    /// </summary>
    private List<PlannedMeld> BuildOpeningPlan()
    {
        PlanState state = new(Me.Hand, _engine.PendingMelds, true);
        List<PlannedMeld> plan = new();
        int points = _engine.PendingPoints;

        points += AddNaturalsToExisting(state, plan);
        points += AddNaturalMelds(state, plan);

        if (points < _engine.OpeningMinimum)
        {
            points += AddPairsWithWilds(state, plan, _engine.OpeningMinimum - points);
        }

        if (points < _engine.OpeningMinimum)
        {
            points += AddSpareWilds(state, plan, _engine.OpeningMinimum - points);
        }

        // A plan may use the whole hand. Before the foot that just picks the foot up; in the foot
        // the engine refuses any meld that would break the going-out rules, and TryMeld then
        // falls back to the melds that remain legal, or to a discard.
        return points < _engine.OpeningMinimum ? new List<PlannedMeld>() : plan;
    }

    /// <summary>
    /// Builds the melds worth making once opened.
    /// </summary>
    private List<PlannedMeld> BuildPlayPlan()
    {
        PlanState state = new(Me.Hand, Me.Melds, false);
        List<PlannedMeld> plan = new();

        AddNaturalsToExisting(state, plan);
        AddNaturalMelds(state, plan);
        AddWildsToFinishBooks(state, plan);

        bool goingOut = Me.HasPickedUpFoot && Me.HasRequiredBooks;
        if (Me.HasPickedUpFoot || state.Wilds.Count >= 2)
        {
            AddPairsWithWilds(state, plan, goingOut ? int.MaxValue : 1);
        }

        if (goingOut)
        {
            AddSpareWilds(state, plan, int.MaxValue);
        }

        return plan;
    }

    private static int AddNaturalsToExisting(PlanState state, List<PlannedMeld> plan)
    {
        int points = 0;
        foreach (Meld meld in state.Existing)
        {
            if (meld.IsBook || !state.Naturals.ContainsKey(meld.Rank))
            {
                continue;
            }

            List<Card> naturals = state.Naturals[meld.Rank];
            int room = GameRules.BOOK_SIZE - state.CountOf(meld);
            int take = Math.Min(room, naturals.Count);
            if (take <= 0)
            {
                continue;
            }

            points += state.Commit(plan, naturals.GetRange(0, take), meld);
            naturals.RemoveRange(0, take);
        }

        return points;
    }

    private static int AddNaturalMelds(PlanState state, List<PlannedMeld> plan)
    {
        int points = 0;
        foreach (Rank rank in state.NaturalRanks())
        {
            List<Card> naturals = state.Naturals[rank];
            if (naturals.Count < GameRules.MINIMUM_MELD_SIZE || state.HasUnfinished(rank))
            {
                continue;
            }

            int take = Math.Min(GameRules.BOOK_SIZE, naturals.Count);
            points += state.Commit(plan, naturals.GetRange(0, take), null);
            naturals.RemoveRange(0, take);
        }

        return points;
    }

    private static int AddPairsWithWilds(PlanState state, List<PlannedMeld> plan, int pointsWanted)
    {
        int points = 0;
        List<Rank> ranks = state.NaturalRanks();
        ranks.Reverse();

        foreach (Rank rank in ranks)
        {
            if (points >= pointsWanted || state.Wilds.Count == 0)
            {
                break;
            }

            List<Card> naturals = state.Naturals[rank];
            if (naturals.Count != 2 || state.HasUnfinished(rank))
            {
                continue;
            }

            List<Card> cards = new(naturals);
            cards.Add(state.TakeWild());
            points += state.Commit(plan, cards, null);
            naturals.Clear();
        }

        return points;
    }

    private void AddWildsToFinishBooks(PlanState state, List<PlannedMeld> plan)
    {
        foreach (Meld meld in state.Existing)
        {
            int count = state.CountOf(meld);
            int need = GameRules.BOOK_SIZE - count;
            if (meld.IsBook || count < 4 || need <= 0 || need > state.Wilds.Count)
            {
                continue;
            }

            int wildsAfter = meld.WildCount + state.PlannedWildsFor(meld) + need;
            int naturalsAfter = count - meld.WildCount - state.PlannedWildsFor(meld);
            if (wildsAfter >= naturalsAfter)
            {
                continue;
            }

            // Keep a meld clean unless a clean book is already secured or a dirty book is urgently needed.
            bool alreadyDirty = meld.WildCount > 0 || state.PlannedWildsFor(meld) > 0;
            bool needDirtyToGoOut = Me.HasPickedUpFoot && Me.DirtyBookCount == 0 && Me.CleanBookCount > 0;
            if (!alreadyDirty && Me.CleanBookCount == 0 && !needDirtyToGoOut)
            {
                continue;
            }

            List<Card> wilds = new();
            for (int i = 0; i < need; i++)
            {
                wilds.Add(state.TakeWild());
            }

            state.Commit(plan, wilds, meld);
        }
    }

    private static int AddSpareWilds(PlanState state, List<PlannedMeld> plan, int pointsWanted)
    {
        int points = 0;
        foreach (PlannedMeld planned in new List<PlannedMeld>(plan))
        {
            points += AddWildsWhileLegal(state, plan, planned.Target, planned, pointsWanted - points);
        }

        foreach (Meld meld in state.Existing)
        {
            points += AddWildsWhileLegal(state, plan, meld, null, pointsWanted - points);
        }

        return points;
    }

    private static int AddWildsWhileLegal(PlanState state, List<PlannedMeld> plan, Meld meld, PlannedMeld newMeld, int pointsWanted)
    {
        int points = 0;
        while (points < pointsWanted && state.Wilds.Count > 0)
        {
            int naturals;
            int wilds;
            if (newMeld != null && newMeld.Target == null)
            {
                naturals = 0;
                wilds = 0;
                foreach (Card card in newMeld.Cards)
                {
                    if (card.IsWild)
                    {
                        wilds++;
                    }
                    else
                    {
                        naturals++;
                    }
                }
            }
            else if (meld != null && newMeld == null)
            {
                if (meld.IsBook)
                {
                    break;
                }

                naturals = meld.NaturalCount + state.PlannedNaturalsFor(meld);
                wilds = meld.WildCount + state.PlannedWildsFor(meld);
            }
            else
            {
                break;
            }

            if (wilds + 1 >= naturals || naturals + wilds + 1 > GameRules.BOOK_SIZE)
            {
                break;
            }

            Card wild = state.TakeWild();
            points += wild.PointValue;
            if (newMeld != null)
            {
                newMeld.Cards.Add(wild);
                state.CardsUsed++;
            }
            else
            {
                state.Commit(plan, new List<Card> { wild }, meld);
            }
        }

        return points;
    }

    private sealed class PlannedMeld
    {
        public PlannedMeld(List<Card> cards, Meld target)
        {
            Cards = cards;
            Target = target;
        }

        public List<Card> Cards { get; }

        /// <summary>
        /// The existing meld to add to, or null for a new meld.
        /// </summary>
        public Meld Target { get; }
    }

    /// <summary>
    /// Working copy of the hand while a plan is assembled.
    /// </summary>
    private sealed class PlanState
    {
        private readonly List<PlannedMeld> _committed = new();

        public PlanState(IList<Card> hand, IList<Meld> existing, bool spendBestWildsFirst)
        {
            Naturals = new Dictionary<Rank, List<Card>>();
            Wilds = new List<Card>();
            Existing = new List<Meld>(existing);

            foreach (Card card in hand)
            {
                if (card.IsWild)
                {
                    Wilds.Add(card);
                }
                else if (card.IsNatural)
                {
                    if (!Naturals.ContainsKey(card.Rank))
                    {
                        Naturals[card.Rank] = new List<Card>();
                    }

                    Naturals[card.Rank].Add(card);
                }
            }

            // Openings need points, so they spend jokers first; otherwise twos are spent first.
            Wilds.Sort(delegate(Card a, Card b)
            {
                int compare = a.PointValue.CompareTo(b.PointValue);
                return spendBestWildsFirst ? -compare : compare;
            });
        }

        public Dictionary<Rank, List<Card>> Naturals { get; }

        public List<Card> Wilds { get; }

        public List<Meld> Existing { get; }

        public int CardsUsed { get; set; }

        public List<Rank> NaturalRanks()
        {
            List<Rank> ranks = new(Naturals.Keys);
            ranks.Sort();
            return ranks;
        }

        public Card TakeWild()
        {
            Card wild = Wilds[0];
            Wilds.RemoveAt(0);
            return wild;
        }

        public bool HasUnfinished(Rank rank)
        {
            foreach (Meld meld in Existing)
            {
                if (meld.Rank == rank && !meld.IsBook)
                {
                    return true;
                }
            }

            foreach (PlannedMeld planned in _committed)
            {
                if (planned.Target == null && RankOf(planned.Cards) == rank)
                {
                    return true;
                }
            }

            return false;
        }

        public int CountOf(Meld meld)
        {
            return meld.Count + PlannedNaturalsFor(meld) + PlannedWildsFor(meld);
        }

        public int PlannedNaturalsFor(Meld meld)
        {
            int count = 0;
            foreach (PlannedMeld planned in _committed)
            {
                if (planned.Target == meld)
                {
                    foreach (Card card in planned.Cards)
                    {
                        if (!card.IsWild)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        public int PlannedWildsFor(Meld meld)
        {
            int count = 0;
            foreach (PlannedMeld planned in _committed)
            {
                if (planned.Target == meld)
                {
                    foreach (Card card in planned.Cards)
                    {
                        if (card.IsWild)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        public int Commit(List<PlannedMeld> plan, List<Card> cards, Meld target)
        {
            PlannedMeld planned = new(new List<Card>(cards), target);
            plan.Add(planned);
            _committed.Add(planned);
            CardsUsed += cards.Count;

            int points = 0;
            foreach (Card card in cards)
            {
                points += card.PointValue;
            }

            return points;
        }

        private static Rank RankOf(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                if (card.IsNatural)
                {
                    return card.Rank;
                }
            }

            return Rank.Joker;
        }
    }
}
