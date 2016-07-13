using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eneo.Model.Models.Entities
{
    public class CollectionItem
    {
        [Key]
        public int CollectionItemID { get; set; }

        public string Name { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime AddedDate { get; set; }
    }
}