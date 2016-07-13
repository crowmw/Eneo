using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eneo.Model.Models.Entities
{
    [XmlInclude(typeof(DMPFacebookLikes))]
    public class DMPFacebook
    {
        [Key]
        public int DMPFacebookID { get; set; }

        [Required]
        public string EneoUserID { get; set; }

        [ForeignKey("EneoUserID")]
        public virtual EneoUser EneoUser { get; set; }

        public long FacebookID { get; set; }

        public string Email { get; set; }

        public string Gender { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Language { get; set; }

        public string Locale { get; set; }

        public int Timezone { get; set; }

        public bool Verified { get; set; }

        public List<DMPFacebookLikes> DMPFacebookLikes { get; set; }
    }
}
