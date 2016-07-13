using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.Model.Models.ViewModels
{
    public class PlacedItemEditModel : PlacedItemModel
    {
        public PlacedItemEditModel (PlacedItemModel placedItem, List<PlacedItemCommentModel> commentsList)
        {
            PlacedItemID = placedItem.PlacedItemID;
            CollectionItemID = placedItem.CollectionItemID;
            Name = placedItem.Name;
            Latitude = placedItem.Latitude;
            Description = placedItem.Description;
            Checked = placedItem.Checked;
            ImageUrl = placedItem.ImageUrl;
            AddedDate = placedItem.AddedDate;
            Longitude = placedItem.Longitude;

            CommentsList = commentsList;
        }
        public List<PlacedItemCommentModel> CommentsList { get; set; }
    }
}
