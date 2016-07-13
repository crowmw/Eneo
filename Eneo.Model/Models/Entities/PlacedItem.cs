using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Eneo.Model.Models.Entities
{
    [XmlInclude(typeof(CollectionItem))]
    public class PlacedItem
    {
        [Key]
        public int PlacedItemID { get; set; }

        public int CollectionItemID { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }

        public string Description { get; set; }

        public Guid ImageGuid { get; set; }

        public DateTime? AddedDate { get; set; }

        public double? Stars { get; set; }

        public virtual CollectionItem CollectionItem { get; set; }

        [XmlIgnore]
        public virtual List<CollectedItem> CollectedItems { get; set; }

        [XmlIgnore]
        public virtual List<PlacedItemComment> PlacedItemComments { get; set; }
    }
}