using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace HeadAndFoot.DesktopApp;

/// <summary>
/// Product text and access to the assets embedded in the executable.
/// </summary>
internal static class AppResources
{
    public const string PRODUCT_NAME = "Head and Foot";
    public const string DESCRIPTION = "The family card game Head and Foot, played one-on-one against the computer.";
    public const string HOMEPAGE = "https://github.com/aaron-salisbury/HeadAndFoot";
    public const string RULES_LINK = "https://officialgamerules.org/game-rules/hand-and-foot/";

    public const string CARD_SHEET = "HeadAndFoot.Assets.Cards.png";
    public const string APP_ICON = "HeadAndFoot.Assets.HeadAndFoot.ico";
    public const string LOGO_ICON = "HeadAndFoot.Assets.HeadAndFootLogo.ico";
    public const string ATTRIBUTIONS = "HeadAndFoot.Assets.Attributions.txt";

    public static Stream Open(string name)
    {
        Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
        if (stream == null)
        {
            throw new InvalidOperationException("Embedded resource '" + name + "' is missing.");
        }

        return stream;
    }

    public static Icon LoadIcon(string name)
    {
        using Stream stream = Open(name);
        return new Icon(stream);
    }

    public static string ReadText(string name)
    {
        using Stream stream = Open(name);
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
