using System;
using System.ComponentModel.DataAnnotations;

namespace Eneo.Model.Models.Entities
{
    public class PlacedItemComment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public int PlacedItemID { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string UserID { get; set; }

        public DateTime AddedDate { get; set; }

        public virtual PlacedItem PlacedItem { get; set; }

        public string EneoUserID { get; set; }

        public virtual EneoUser EneoUser { get; set; }
    }
}