using Eneo.Model.Models.Entities;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Repository.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Eneo.Model.Repository
{
    public class PlacedItemsRepo : EneoRepository
    {
        private string _imagePath;

        public PlacedItemsRepo(EneoContext context)
            : base(context)
        {
            _imagePath = "http://eneo.azurewebsites.net/api/Map/GetImage/";
            _eneoCtx = context;
        }

        #region MapItems

        public IQueryable<PlacedItemModel> GetMapItems(string userID, string range)
        {
            IQueryable<PlacedItemModel> items = _eneoCtx.PlacedItems.LeftOuterJoin(_eneoCtx.CollectedItems.Where(u => u.EneoUserID == userID),
                 pl => pl.PlacedItemID,
                 co => co.PlacedItemID,
                 (pl, co) => new PlacedItemModel
                 {
                     Name = pl.CollectionItem.Name,
                     CollectionItemID = pl.CollectionItemID,
                     Latitude = pl.Latitude,
                     Longitude = pl.Longitude,
                     PlacedItemID = pl.PlacedItemID,
                     Checked = co.EneoUserID == userID ? true : false,
                     Description = pl.Description,
                     AddedDate = pl.AddedDate,
                     ImageUrl = string.IsNullOrEmpty(pl.ImageGuid.ToString()) ? string.Empty : pl.ImageGuid == Guid.Empty ? string.Empty : _imagePath + pl.ImageGuid.ToString()
                 });
            return items;
        }

        public IQueryable<PlacedItemModel> GetMapItems()
        {
            return _eneoCtx.PlacedItems.Select(i => new PlacedItemModel
                {
                    Name = i.CollectionItem.Name,
                    CollectionItemID = i.CollectionItemID,
                    Latitude = i.Latitude,
                    Longitude = i.Longitude,
                    PlacedItemID = i.PlacedItemID,
                    Description = i.Description,
                    AddedDate = i.AddedDate,
                    ImageUrl = string.IsNullOrEmpty(i.ImageGuid.ToString()) ? string.Empty : i.ImageGuid == Guid.Empty ? string.Empty : _imagePath + i.ImageGuid.ToString()
                });
        }

        public PlacedItemModel AddMapItem(PlacedItemModel item)
        {
            PlacedItem placedItem = GetPlacedItem(item);
            _eneoCtx.PlacedItems.Add(placedItem);
            _eneoCtx.SaveChanges();
            //_eneoCtx.PlacedItems.Where(u => u.CollectionItemID == placedItem.CollectionItemID).Include(g => g.CollectedItems).Single();
            return GetPlacedItemModel(placedItem);
        }

        public void AddWikiPlaces(IEnumerable<PlacedItemWiki> places)
        {
            var dbPlaces = places.Select(n => new PlacedItem
                {
                    AddedDate = DateTime.Now,
                    CollectionItem = new CollectionItem
                    {
                        AddedDate = DateTime.Now,
                        Name = n.Name
                    },
                    Description = n.Description,
                    ImageGuid = new Guid(n.ImageUrl),
                    Latitude = n.Latitude.Replace(',', '.'),
                    Longitude = n.Longitude.Replace(',', '.'),
                    Stars = 0
                });

            _eneoCtx.PlacedItems.AddRange(dbPlaces);
            _eneoCtx.SaveChanges();
        }

        public PlacedItemModel UpdateMapItem(PlacedItemModel item)
        {
            PlacedItem placedItem = GetPlacedItem(item);
            _eneoCtx.PlacedItems.Attach(placedItem);
            var entry = _eneoCtx.Entry(placedItem);
            entry.State = EntityState.Modified;
            _eneoCtx.SaveChanges();
            item.ImageUrl = _imagePath + placedItem.ImageGuid;
            return item;
        }

        public void DeleteMapItem(int id)
        {
            PlacedItem item = new PlacedItem { PlacedItemID = id };
            _eneoCtx.Entry(item).State = EntityState.Deleted;
            _eneoCtx.SaveChangesAsync();
        }

        public PlacedItemModel GetMapItem(int placedItemID)
        {
            var item = _eneoCtx.PlacedItems.Single(i => i.PlacedItemID == placedItemID);
            PlacedItemModel mapPlacedItem = GetPlacedItemModel(item);

            return mapPlacedItem;
        }

        #endregion MapItems

        #region Comments

        public IQueryable<PlacedItemCommentModel> GetComments(int placedItemID)
        {
            var comments = _eneoCtx.PlacedItemComments
                .Where(c => c.PlacedItemID == placedItemID)
                .Select(c => new PlacedItemCommentModel
                {
                    AddedDate = c.AddedDate,
                    CommentID = c.CommentID,
                    Content = c.Content,
                    PlacedItemID = c.PlacedItemID,
                    UserID = c.UserID,
                    UserName = c.EneoUser.Name
                });

            return comments;
        }

        public void DeletePlacedItemComment(int id)
        {
            PlacedItemComment item = new PlacedItemComment { CommentID = id };
            _eneoCtx.PlacedItemComments.Attach(item);
            _eneoCtx.PlacedItemComments.Remove(item);
            _eneoCtx.SaveChanges();
        }

        public IQueryable<PlacedItemCommentModel> GetUserComments(int placedItemID, string UserID)
        {
            var comments = _eneoCtx.PlacedItemComments
                .Where(c => c.PlacedItemID == placedItemID && c.UserID == UserID)
                .Select(c => new PlacedItemCommentModel
                {
                    AddedDate = c.AddedDate,
                    Content = c.Content,
                    CommentID = c.CommentID,
                    UserName = c.EneoUser.Name,
                    PlacedItemID = c.PlacedItemID,
                    UserID = c.UserID
                });

            return comments;
        }

        public bool AddComment(PlacedItemCommentModel comment)
        {
            var dbComment = new PlacedItemComment
            {
                AddedDate = DateTime.Now,
                Content = comment.Content,
                UserID = comment.UserID,
                EneoUserID = comment.UserID,
                PlacedItemID = comment.PlacedItemID
            };

            _eneoCtx.PlacedItemComments.Add(dbComment);
            _eneoCtx.SaveChangesAsync();
            return true;
        }

        public void EditComment(PlacedItemCommentModel comment)
        {
            var dbComment = _eneoCtx.PlacedItemComments.SingleOrDefault(i => i.UserID == comment.UserID && i.PlacedItemID == comment.PlacedItemID);

            if (dbComment == null)
                return;

            dbComment.AddedDate = DateTime.Now;
            dbComment.Content = comment.Content;
            _eneoCtx.SaveChangesAsync();
        }

        #endregion Comments

        #region Helpers

        private PlacedItem GetPlacedItem(PlacedItemModel item)
        {

            PlacedItem placedItem;
            if (item.PlacedItemID != 0)
            {
                placedItem = _eneoCtx.PlacedItems.Single(i => i.PlacedItemID == item.PlacedItemID);
            }
            else
            {
                placedItem = _eneoCtx.PlacedItems.Create();
            }

            placedItem.CollectionItemID = item.CollectionItemID;
            placedItem.Latitude = item.Latitude;
            placedItem.Longitude = item.Longitude;
            placedItem.Description = item.Description;
            placedItem.ImageGuid = string.IsNullOrEmpty(item.ImageUrl) ? placedItem.ImageGuid != Guid.Empty ? placedItem.ImageGuid : Guid.Empty : new Guid(item.ImageUrl);
            placedItem.AddedDate = item.AddedDate.HasValue ? item.AddedDate : DateTime.Now;
            return placedItem;
        }

        private PlacedItemModel GetPlacedItemModel(PlacedItem item)
        {
            PlacedItemModel mapPlacedItem = new PlacedItemModel
            {
                Name = item.CollectionItem.Name,
                PlacedItemID = item.PlacedItemID,
                CollectionItemID = item.CollectionItemID,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                Description = item.Description,
                AddedDate = item.AddedDate,
                ImageUrl = _imagePath + item.ImageGuid.ToString()
            };
            return mapPlacedItem;
        }

        #endregion Helpers
    }
}