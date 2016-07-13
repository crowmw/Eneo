using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eneo.Model.Models.Entities
{
    public class CollectedItem
    {
        [Key]
        public int CollectedItemID { get; set; }

        [Required]
        public string EneoUserID { get; set; }

        [Required]
        public int PlacedItemID { get; set; }

        public virtual EneoUser EneoUser { get; set; }

        [ForeignKey("PlacedItemID")]
        public virtual PlacedItem PlacedItem { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CatchDate { get; set; }
    }
}