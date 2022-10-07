using HeadAndFoot.Core.Data;
using System.Collections.Generic;
using System.Drawing;

namespace HeadAndFoot.Core
{
    public  class Manager
    {
        public Dictionary<string, Image> CardBackSprites { get; set; }

        public Manager()
        {
            CardBackSprites = CRUD.ReadCardBackSprites();

        }
    }
}
