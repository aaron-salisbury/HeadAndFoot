using AmalgamateLabs.MCGE.DataAccess;
using System.Collections.Generic;
using System.Drawing;

namespace AmalgamateLabs.MCGE.Business
{
    public class Cards
    {
        private Dictionary<string, Image> _backs;
        public Dictionary<string, Image> Backs
        {
            get { return _backs; }
            set { _backs = value; }
        }

        public Cards()
        {
            Backs = CRUD.ReadCardBackSprites();
        }
    }
}
