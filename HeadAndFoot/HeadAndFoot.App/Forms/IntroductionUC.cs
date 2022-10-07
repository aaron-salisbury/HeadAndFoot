using System.Windows.Forms;

namespace HeadAndFoot.App.Forms
{
    public partial class IntroductionUC : UserControl
    {
        public IntroductionUC()
        {
            InitializeComponent();

            ShellForm.GameManager.NewDeal();
        }
    }
}
