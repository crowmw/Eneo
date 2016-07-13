using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eneo.Model.Models.Entities
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Email { get; set; }

        public string NIP { get; set; }

        public string UserManagerID { get; set; }

        [ForeignKey("UserManagerID")]
        public virtual EneoUser UserManager { get; set; }
    }
}