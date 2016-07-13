using System;

namespace Eneo.WindowsPhone.DataModel
{
    public class PlacedItemModel
    {
        public int PlacedItemID { get; set; }

        public int CollectionItemID { get; set; }

        public string Name { get; set; }

        public string Latitude { get; set; }

        public string Description { get; set; }

        public bool Checked { get; set; }

        public string ImageUrl { get; set; }

        public DateTime AddedDate { get; set; }

        public string Longitude { get; set; }

        public bool Unvisited { get; set; }

        public PlacedItemModel()
        {
            Unvisited = false;
        }
    }
}