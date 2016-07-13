using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.WindowsPhone.DataModel
{
    public class CollectedItemModel
    {
        public int CollectedItemID { get; set; }
        public int PlacedItemID { get; set; }

        public string Name { get; set; }
        public DateTime CatchDate { get; set; }

        public string EneoUserID { get; set; }

        public string UserName { get; set; }

    }
}
