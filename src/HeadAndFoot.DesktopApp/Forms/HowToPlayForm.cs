using HeadAndFoot.Business.Game;
using System;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp.Forms;

/// <summary>
/// A short guide to the rules and to playing with the mouse.
/// </summary>
internal partial class HowToPlayForm : Form
{
    public HowToPlayForm()
    {
        InitializeComponent();
        Text = "How to Play " + AppResources.PRODUCT_NAME;
        GuideTextBox.Text = BuildGuide();
        GuideTextBox.Select(0, 0);
    }

    private static string BuildGuide()
    {
        string nl = Environment.NewLine;
        return
            "OBJECT OF THE GAME" + nl +
            "Score the most points over " + GameRules.ROUND_COUNT + " rounds by laying down melds of matching cards and completing books of seven." + nl + nl +
            "THE DEAL" + nl +
            "Five decks with jokers are shuffled together. You and the computer each get an 11-card hand and an 11-card foot. " +
            "The foot stays face down until your hand is gone." + nl + nl +
            "YOUR TURN" + nl +
            "1. Draw: click the stock to draw two cards, or click the discard pile to pick it up." + nl +
            "2. Meld (optional): click cards in your hand to select them, then click your meld area (just above your hand) to start a new meld, " +
            "or click one of your melds to add to it. Right-click or press Esc to clear the selection." + nl +
            "3. Discard: select one card and click the discard pile, or double-click the card." + nl + nl +
            "MELDS AND BOOKS" + nl +
            "A meld is three or more cards of the same rank. Twos and jokers are wild, but natural cards must always outnumber wild cards. " +
            "Threes cannot be melded. Seven cards make a book: a clean book (no wild cards) is worth 500 bonus points, a dirty book 300." + nl + nl +
            "OPENING" + nl +
            "Your first melds each round must total at least 50 points in round 1, 90 in round 2, 120 in round 3, and 150 in round 4. " +
            "Melds laid down toward an opening are outlined until they reach the minimum; click an outlined meld to take it back." + nl + nl +
            "THE DISCARD PILE" + nl +
            "You may take the top card and the two beneath it (they are fanned so you can see them) if you hold two natural cards matching the top card, which are melded with it right away. " +
            "The pile cannot be taken while a three or a wild card is on top. " +
            "When the stock runs out, the discards under the top card are shuffled into a new stock." + nl + nl +
            "RED AND BLACK THREES" + nl +
            "Threes cannot be melded, so discard them. A red three still in your hand or foot at the end of a round costs 500 points. " +
            "Black threes are good discards too, since any three on top blocks the pile." + nl + nl +
            "THE FOOT AND GOING OUT" + nl +
            "When your hand is empty you pick up your foot. If you melded your last card you keep playing; if you discarded it, your turn ends. " +
            "To go out you must be playing from your foot, have at least one clean and one dirty book, and meld your final card; you cannot go out on a discard. " +
            "Until then, keep at least two cards so you can discard one and hold the other. Going out is worth 100 points and ends the round." + nl + nl +
            "SCORING" + nl +
            "Jokers 50; twos and aces 20; eights through kings 10; fours through sevens 5; black threes 5; red threes 500. Melded cards count for you; cards left in your hand and foot count against you.";
    }
}
