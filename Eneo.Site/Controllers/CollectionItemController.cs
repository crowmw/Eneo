using Eneo.Model;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Repository;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;

namespace Eneo.Site.Controllers
{
    [Authorize]
    public class CollectionItemController : Controller
    {
        private EneoContext _eneoCtx;
        private CollectionItemsRepo _collectionItemsRepo;

        public CollectionItemController()
        {
            _eneoCtx = new EneoContext();
            _collectionItemsRepo = new CollectionItemsRepo(_eneoCtx);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            return View();
        }

        public ActionResult CollectionItems_Read([DataSourceRequest] DataSourceRequest request, CollectionItemModel model)
        {
            var items = _collectionItemsRepo.GetCollectionItems();
            return Json(items.ToDataSourceResult(request));
        }

        public ActionResult CollectionItem_Create([DataSourceRequest] DataSourceRequest request, CollectionItemModel model)
        {
            if (ModelState.IsValid)
            {
                _collectionItemsRepo.AddCollectionItem(model);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult CollectionItem_Update([DataSourceRequest] DataSourceRequest request, CollectionItemModel model)
        {
            if (ModelState.IsValid)
            {
                _collectionItemsRepo.UpdateCollectionItem(model);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult CollectionItem_Delete([DataSourceRequest] DataSourceRequest request, CollectionItemModel model)
        {
            if (ModelState.IsValid)
            {
                _collectionItemsRepo.DeleteCollectionItem(model.CollectionItemID);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}