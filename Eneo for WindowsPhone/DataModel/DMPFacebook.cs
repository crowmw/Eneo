using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.WindowsPhone.DataModel
{
    public class DMPFacebook
    {
        public string EneoUserID { get; set; }

        public virtual Profile EneoUser { get; set; }

        public string FacebookID { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Language { get; set; }

        public string Locale { get; set; }

        public string Timezone { get; set; }

        public string Verified { get; set; }

        public ICollection<DMPFacebookLikes> DMPFacebookLikes { get; set; }
    }
}
