using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.Model.Models.ViewModels.User
{
    public class NearestUser
    {
        public string UserID { get; set; }

        public string UserName { get; set; }

        public double Distance { get; set; }

        public double? LastLatitude { get; set; }
        public double? LastLongitude { get; set; }
    }
}
