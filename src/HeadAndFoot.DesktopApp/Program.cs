using HeadAndFoot.Business.Game;
using System;
using System.Windows.Forms;

namespace HeadAndFoot.DesktopApp;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        GameEngine engine = new(new Random());
        using ShellForm shell = new(engine);
        Application.Run(shell);
    }
}
