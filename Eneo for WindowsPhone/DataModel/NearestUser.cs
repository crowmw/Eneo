using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.WindowsPhone.DataModel
{
    public class NearestUser
    {
        public string userID { get; set; }
        public string userName { get; set; }
        public float distance { get; set; }
        public float? lastLatitude { get; set; }
        public float? lastLongitude { get; set; }
    }
}
