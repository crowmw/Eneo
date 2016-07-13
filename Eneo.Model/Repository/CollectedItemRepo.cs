using Eneo.Model.Models.Entities;
using Eneo.Model.Models.ViewModels;
using System;
using System.Linq;

namespace Eneo.Model.Repository
{
    public class CollectedItemsRepo : EneoRepository
    {
        public CollectedItemsRepo(EneoContext context)
            : base(context)
        {
            _eneoCtx = context;
        }

        public bool IsChecked(int mapPlacedItemID, string userID)
        {
            var placedItem = _eneoCtx.CollectedItems.SingleOrDefault(i => i.EneoUserID == userID && i.PlacedItemID == mapPlacedItemID);
            if (placedItem == null)
                return false;

            return true;
        }

        public void AddCheckin(int mapPlacedItemID, string userID)
        {
            CollectedItem item = new CollectedItem
            {
                CatchDate = DateTime.Now,
                EneoUserID = userID,
                PlacedItemID = mapPlacedItemID
            };
            _eneoCtx.CollectedItems.Add(item);
            _eneoCtx.SaveChangesAsync();
        }

        public IQueryable<CollectedItemModel> GetUserCollectedItems(string userID)
        {
            var collectedItems = _eneoCtx.CollectedItems.Where(i => i.EneoUserID == userID)
                .Select(n => new CollectedItemModel
                  {
                      CatchDate = n.CatchDate,
                      Name = n.PlacedItem.CollectionItem.Name,
                      UserName = n.EneoUser.Name,
                      CollectedItemID = n.CollectedItemID,
                      PlacedItemID = n.PlacedItemID,
                  });

            return collectedItems;
        }
    }
}