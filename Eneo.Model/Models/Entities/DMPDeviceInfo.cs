using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Eneo.Model.Models.Entities
{
    public class DMPDeviceInfo
    {
        [Key]
        public int DMPDeviceInfoID { get; set; }

        public string EneoUserID { get; set; }

        public string OperatingSystem { get; set; }

        public string SystemFirmwareVersion { get; set; }

        public string SystemHardwareVersion { get; set; }

        public string SystemManufacturer { get; set; }

        public string SystemProductName { get; set; }

        //[XmlIgnore]
        public virtual EneoUser EneoUser { get; set; }
    }
}
