using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Eneo.Model.Models.Entities
{
    //[XmlInclude(typeof(List<CollectedItem>))]
    public class EneoUser
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string EneoUserID { get; set; }

        public string Name { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? RegisterDate { get; set; }

        public Guid? AvatarGuid { get; set; }

        public string UserIDFromProvider { get; set; }

        public string Provider { get; set; }

        public string DisplayName { get; set; }

        [DefaultValue(0)]
        public double? LastLatitude { get; set; }

        [DefaultValue(0)]
        public double? LastLongitude { get; set; }

        [XmlIgnore]
        public virtual List<CollectedItem> CollectedItems { get; set; }

        [XmlIgnore]
        public virtual List<PlacedItemComment> PlacedItemComments { get; set; }

        [XmlIgnore]
        public virtual List<DMPFacebook> DMPFacebook { get; set; }

        public EneoUser()
        {

        }
    }
}