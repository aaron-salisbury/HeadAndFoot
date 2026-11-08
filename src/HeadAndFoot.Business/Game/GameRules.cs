namespace HeadAndFoot.Business.Game;

/// <summary>
/// Fixed rule values for a two-player game, following the rules published at
/// https://officialgamerules.org/game-rules/hand-and-foot/ (11-card hand and foot,
/// four rounds), with house rules: at least five decks are used; red threes are held
/// and discarded like other cards; a player goes out by melding their last card rather
/// than discarding it; and an exhausted stock is rebuilt from the discards.
/// </summary>
public static class GameRules
{
    public const int PLAYER_COUNT = 2;

    /// <summary>
    /// The published rule of one deck more than the number of players leaves a two-player
    /// game short of cards, so at least this many decks are used.
    /// </summary>
    public const int MINIMUM_DECK_COUNT = 5;

    public const int DECK_COUNT = PLAYER_COUNT + 1 > MINIMUM_DECK_COUNT ? PLAYER_COUNT + 1 : MINIMUM_DECK_COUNT;
    public const int HAND_SIZE = 11;
    public const int FOOT_SIZE = 11;
    public const int MINIMUM_MELD_SIZE = 3;
    public const int BOOK_SIZE = 7;
    public const int CARDS_DRAWN_FROM_STOCK = 2;
    /// <summary>
    /// Picking up the discard pile takes the top card and the two beneath it.
    /// </summary>
    public const int MAXIMUM_PICKUP = 3;
    public const int NATURALS_NEEDED_FOR_PICKUP = 2;
    public const int ROUND_COUNT = 4;
    public const int CLEAN_BOOK_BONUS = 500;
    public const int DIRTY_BOOK_BONUS = 300;
    public const int GOING_OUT_BONUS = 100;

    /// <summary>
    /// The minimum printed card value a player must lay down to open in a round.
    /// </summary>
    public static int OpeningMinimum(int round)
    {
        switch (round)
        {
            case 1:
                return 50;
            case 2:
                return 90;
            case 3:
                return 120;
            default:
                return 150;
        }
    }
}
