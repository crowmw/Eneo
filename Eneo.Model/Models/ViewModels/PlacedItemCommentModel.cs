using System;
namespace Eneo.Model.Models.ViewModels
{
    public class PlacedItemCommentModel
    {
        public int CommentID { get; set; }

        public int PlacedItemID { get; set; }

        public string Content { get; set; }

        public string UserID { get; set; }

        public string UserName { get; set; }

        public DateTime AddedDate { get; set; }
    }
}