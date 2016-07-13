using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Eneo.Model.Models.Entities
{
    public class DMPUserPosition
    {
        [Key]
        public int DMPUserPositionID { get; set; }

        public string EneoUserID { get; set; }

        public DateTime Date { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        [XmlIgnore]
        public virtual EneoUser EneoUser { get; set; }
    }
}