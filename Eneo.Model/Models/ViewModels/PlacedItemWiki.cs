using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.Model.Models.ViewModels
{
    public class PlacedItemWiki
    {
        public int PlacedItemID { get; set; }

        public int CollectionItemID { get; set; }

        public string Name { get; set; }

        public string Latitude { get; set; }

        public string Description { get; set; }

        public bool Checked { get; set; }

        public string ImageUrl { get; set; }

        public DateTime? AddedDate { get; set; }

        public string Longitude { get; set; }

        public bool Add { get; set; }
    }
}
