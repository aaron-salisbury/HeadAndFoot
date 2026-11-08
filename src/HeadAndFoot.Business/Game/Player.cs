using HeadAndFoot.Business.Cards;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HeadAndFoot.Business.Game;

/// <summary>
/// One side of the table: the cards held, the face-down foot, and everything laid down.
/// </summary>
public sealed class Player
{
    private readonly List<Card> _hand = new();
    private readonly List<Card> _foot = new();
    private readonly List<Meld> _melds = new();
    private readonly List<RoundScore> _roundScores = new();

    internal Player(string name, bool isComputer)
    {
        Name = name;
        IsComputer = isComputer;
        Hand = _hand.AsReadOnly();
        Foot = _foot.AsReadOnly();
        Melds = _melds.AsReadOnly();
        RoundScores = _roundScores.AsReadOnly();
    }

    public string Name { get; }

    public bool IsComputer { get; }

    /// <summary>
    /// The cards currently held: the hand, or the foot once it has been picked up.
    /// </summary>
    public ReadOnlyCollection<Card> Hand { get; }

    /// <summary>
    /// The face-down foot. It is empty once picked up.
    /// </summary>
    public ReadOnlyCollection<Card> Foot { get; }

    public bool HasPickedUpFoot { get; internal set; }

    public bool HasOpened { get; internal set; }

    public ReadOnlyCollection<Meld> Melds { get; }

    public ReadOnlyCollection<RoundScore> RoundScores { get; }

    public int TotalScore
    {
        get
        {
            int total = 0;
            foreach (RoundScore score in _roundScores)
            {
                total += score.Total;
            }

            return total;
        }
    }

    public int CleanBookCount
    {
        get
        {
            int count = 0;
            foreach (Meld meld in _melds)
            {
                if (meld.IsBook && meld.IsClean)
                {
                    count++;
                }
            }

            return count;
        }
    }

    public int DirtyBookCount
    {
        get
        {
            int count = 0;
            foreach (Meld meld in _melds)
            {
                if (meld.IsBook && !meld.IsClean)
                {
                    count++;
                }
            }

            return count;
        }
    }

    /// <summary>
    /// True when the player has the books required to go out.
    /// </summary>
    public bool HasRequiredBooks => CleanBookCount >= 1 && DirtyBookCount >= 1;

    /// <summary>
    /// Returns the player's unfinished meld of the rank, or null.
    /// </summary>
    public Meld FindOpenMeld(Rank rank)
    {
        foreach (Meld meld in _melds)
        {
            if (meld.Rank == rank && !meld.IsBook)
            {
                return meld;
            }
        }

        return null;
    }

    internal List<Card> HandList => _hand;

    internal List<Card> FootList => _foot;

    internal List<Meld> MeldList => _melds;

    internal List<RoundScore> RoundScoreList => _roundScores;

    internal void ResetForRound()
    {
        _hand.Clear();
        _foot.Clear();
        _melds.Clear();
        HasPickedUpFoot = false;
        HasOpened = false;
    }
}
