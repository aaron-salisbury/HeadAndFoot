using AmalgamateLabs.MCGE.Business;
using HeadAndFoot.App.Forms;
using HeadAndFoot.Core;
using HeadAndFoot.Core.Base.Logging;
using System.Windows.Forms;

namespace HeadAndFoot.App
{
    public partial class ShellForm : AmalgamateLabs.MCGE.Presentation.ShellForm
    {
        public static Manager GameManager;

        private readonly Padding _defaultContentAreaPadding = new Padding(5);
        private readonly LogUC _logUC;

        public ShellForm(GameConfig gameConfig) : base(gameConfig)
        {
            InitializeComponent();

            NewDealUCMethodToCall = GetNewDeal;

            //_logUC = new LogUC();
            //EstablishLogging();

            //GameManager = new Manager();
        }

        private void EstablishLogging()
        {
            _logUC.Padding = _defaultContentAreaPadding;
            _logUC.Dock = DockStyle.Fill;
            AppLogger.SetTargetInvoking(_logUC.UpdateLogs);
            _logUC.UpdateLogs(AppLogger.GetLogs()); // Load logs that may have been written before delegate could be set.
        }

        private UserControl GetNewDeal()
        {
            UserControl homeContent = new GameBoardUC();
            homeContent.Padding = _defaultContentAreaPadding;
            homeContent.Dock = DockStyle.Fill;

            return homeContent;
        }
    }
}
