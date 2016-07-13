using Eneo.Model.Models.Entities;
using Eneo.Model.Models.ViewModels;
using System.Data.Entity;
using System.Linq;

namespace Eneo.Model.Repository
{
    public class CollectionItemsRepo : EneoRepository
    {
        public CollectionItemsRepo(EneoContext context)
            : base(context)
        {
            _eneoCtx = context;
        }

        public IQueryable<CollectionItemModel> GetCollectionItems()
        {
            return _eneoCtx.CollectionItems.Select(n => new CollectionItemModel
                 {
                     AddedDate = n.AddedDate,
                     CollectionItemID = n.CollectionItemID,
                     Name = n.Name
                 });
        }

        public void AddCollectionItem(CollectionItemModel model)
        {
            CollectionItem item = GetCollectionItem(model);
            _eneoCtx.CollectionItems.Add(item);
            _eneoCtx.SaveChangesAsync();
        }

        public void UpdateCollectionItem(CollectionItemModel model)
        {
            CollectionItem item = GetCollectionItem(model);
            _eneoCtx.CollectionItems.Attach(item);
            _eneoCtx.Entry(item).State = EntityState.Modified;
            _eneoCtx.SaveChangesAsync();
        }

        public void DeleteCollectionItem(int id)
        {
            CollectionItem item = new CollectionItem { CollectionItemID = id };
            _eneoCtx.Entry(item).State = EntityState.Deleted;
            _eneoCtx.SaveChangesAsync();
        }
        public CollectionItemModel GetCollectionItem (int id)
        {
            var collectionItem = _eneoCtx.CollectionItems.Single(i => i.CollectionItemID == id);
            CollectionItemModel model = new CollectionItemModel
            {
                AddedDate = collectionItem.AddedDate,
                CollectionItemID = collectionItem.CollectionItemID,
                Name = collectionItem.Name
            };
            return model;
        }

        private CollectionItem GetCollectionItem(CollectionItemModel model)
        {
            CollectionItem item = new CollectionItem
            {
                AddedDate = model.AddedDate,
                CollectionItemID = model.CollectionItemID,
                Name = model.Name
            };
            return item;
        }

    }
}