using System.Windows.Forms;

namespace HeadAndFoot.App.Forms
{
    public partial class GameBoardUC : UserControl
    {
        public GameBoardUC()
        {
            InitializeComponent();

            ShellForm.GameManager.NewDeal();
        }
    }
}
