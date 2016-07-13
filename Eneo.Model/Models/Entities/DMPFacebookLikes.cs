using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.Model.Models.Entities
{
    public class DMPFacebookLikes
    {
        [Key]
        public int DMPFacebookLikesID { get; set; }

        [Required]
        public int DMPFacebookID { get; set; }

        [ForeignKey("DMPFacebookID")]
        public virtual DMPFacebook DMPFacebook { get; set; }

        public string CategoryName { get; set; }

        public string LikeName { get; set; }

        public string LikeID { get; set; }

        public DateTime AddedDate { get; set; }

    }
}
