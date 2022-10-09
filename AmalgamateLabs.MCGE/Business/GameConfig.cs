using System;
using System.Drawing;

namespace AmalgamateLabs.MCGE.Business
{
    [Serializable]
    public class GameConfig
    {
        private Icon _appIcon;
        public Icon AppIcon
        {
            get { return _appIcon; }
            set { _appIcon = value; }
        }

        private Image _appLogoImage;
        public Image AppLogoImage
        {
            get { return _appLogoImage; }
            set { _appLogoImage = value; }
        }

        private string _productVersion;
        public string ProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        private string _applicationFriendlyName;
        public string ApplicationFriendlyName
        {
            get { return _applicationFriendlyName; }
            set { _applicationFriendlyName = value; }
        }

        private string _aboutDescription;
        public string AboutDescription
        {
            get { return _aboutDescription; }
            set { _aboutDescription = value; }
        }

        private string _applicationLink;
        public string ApplicationLink
        {
            get { return _applicationLink; }
            set { _applicationLink = value; }
        }

        private string _aboutCopyrightHolder;
        public string AboutCopyrightHolder
        {
            get { return _aboutCopyrightHolder; }
            set { _aboutCopyrightHolder = value; }
        }

        private string _aboutLeadProgrammerName;
        public string AboutLeadProgrammerName
        {
            get { return _aboutLeadProgrammerName; }
            set { _aboutLeadProgrammerName = value; }
        }

        private string _gameRulesLink;
        public string GameRulesLink
        {
            get { return _gameRulesLink; }
            set { _gameRulesLink = value; }
        }
    }
}
