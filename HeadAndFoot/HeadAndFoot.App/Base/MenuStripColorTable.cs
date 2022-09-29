using System.Drawing;
using System.Windows.Forms;

namespace HeadAndFoot.App.Base
{
    internal class MenuStripColorTable : ProfessionalColorTable
    {
        // stackoverflow.com/questions/36767478/color-change-for-menuitem

        private readonly static Color MENU_COLOR = System.Drawing.SystemColors.Control;
        private readonly static Color MENU_TOP_HOVER_COLOR = System.Drawing.SystemColors.ControlLight;
        private readonly static Color MENU_ITEMS_HOVER_COLOR = System.Drawing.SystemColors.ControlLight;

        public override Color ToolStripDropDownBackground
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color ImageMarginGradientBegin
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color ImageMarginGradientMiddle
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color MenuBorder
        {
            get
            {
                return System.Drawing.SystemColors.ControlDark;
            }
        }

        public override Color MenuItemBorder
        {
            get
            {
                return System.Drawing.Color.Transparent;
            }
        }

        public override Color MenuItemSelected
        {
            get
            {
                return MENU_ITEMS_HOVER_COLOR;
            }
        }

        public override Color MenuStripGradientBegin
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color MenuStripGradientEnd
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                return MENU_TOP_HOVER_COLOR;
            }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                return MENU_TOP_HOVER_COLOR;
            }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                return MENU_COLOR;
            }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                return MENU_COLOR;
            }
        }
    }
}
