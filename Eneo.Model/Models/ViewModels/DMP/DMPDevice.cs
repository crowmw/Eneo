using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eneo.Model.Models.ViewModels.DMP
{
    public class DMPDevice
    {
        public string OperatingSystem { get; set; }

        public string SystemFirmwareVersion { get; set; }

        public string SystemHardwareVersion { get; set; }

        public string SystemManufacturer { get; set; }

        public string SystemProductName { get; set; }
    }
}
