namespace HeadAndFoot.Business.Game;

/// <summary>
/// A player's score breakdown for one round.
/// </summary>
public sealed class RoundScore
{
    public RoundScore(int round, int meldPoints, int bookBonus, int goingOutBonus, int cardsLeftPenalty)
    {
        Round = round;
        MeldPoints = meldPoints;
        BookBonus = bookBonus;
        GoingOutBonus = goingOutBonus;
        CardsLeftPenalty = cardsLeftPenalty;
    }

    public int Round { get; }

    public int MeldPoints { get; }

    public int BookBonus { get; }

    public int GoingOutBonus { get; }

    /// <summary>
    /// The value of cards still held in hand or foot, expressed as a positive number to subtract.
    /// Red threes count 500 each.
    /// </summary>
    public int CardsLeftPenalty { get; }

    public int Total => MeldPoints + BookBonus + GoingOutBonus - CardsLeftPenalty;
}
