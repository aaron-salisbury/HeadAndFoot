namespace HeadAndFoot.Business.Cards;

/// <summary>
/// One physical card. Several decks are shuffled together, so each card carries an
/// <see cref="Id"/> that distinguishes otherwise identical cards.
/// </summary>
public sealed class Card
{
    public Card(int id, Suit suit, Rank rank)
    {
        Id = id;
        Suit = suit;
        Rank = rank;
    }

    public int Id { get; }

    public Suit Suit { get; }

    public Rank Rank { get; }

    public bool IsJoker => Rank == Rank.Joker;

    public bool IsWild => Rank == Rank.Two || Rank == Rank.Joker;

    public bool IsRed => Suit == Suit.Diamonds || Suit == Suit.Hearts;

    public bool IsThree => Rank == Rank.Three;

    public bool IsRedThree => IsThree && IsRed;

    public bool IsBlackThree => IsThree && !IsRed;

    /// <summary>
    /// A natural card can anchor a meld: anything other than a wild card or a three.
    /// </summary>
    public bool IsNatural => !IsWild && !IsThree;

    /// <summary>
    /// The printed value of the card when melded or left in hand. A red three cannot be
    /// melded; its value only counts against a player caught holding it.
    /// </summary>
    public int PointValue
    {
        get
        {
            switch (Rank)
            {
                case Rank.Joker:
                    return 50;
                case Rank.Two:
                case Rank.Ace:
                    return 20;
                case Rank.Eight:
                case Rank.Nine:
                case Rank.Ten:
                case Rank.Jack:
                case Rank.Queen:
                case Rank.King:
                    return 10;
                case Rank.Three:
                    return IsRed ? 500 : 5;
                default:
                    return 5;
            }
        }
    }

    public override string ToString()
    {
        if (IsJoker)
        {
            return "Joker";
        }

        return RankName(Rank) + " of " + Suit;
    }

    public static string RankName(Rank rank)
    {
        switch (rank)
        {
            case Rank.Ace:
                return "Ace";
            case Rank.King:
                return "King";
            case Rank.Queen:
                return "Queen";
            case Rank.Jack:
                return "Jack";
            case Rank.Joker:
                return "Joker";
            default:
                return ((int)rank).ToString();
        }
    }

    public static string RankPluralName(Rank rank)
    {
        switch (rank)
        {
            case Rank.Six:
                return "Sixes";
            default:
                return RankName(rank) + "s";
        }
    }
}
