using Eneo.Helpers;
using Eneo.Model;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Repository;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using CsQuery;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using System.IO;
using Eneo.Model.Models.Entities;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace Eneo.Site.Controllers
{
    [Authorize]
    public class PlacedItemController : Controller
    {
        private EneoContext _eneoCtx;
        private PlacedItemsRepo _placedItemsRepo;
        private CollectedItem _collectedItem;
        private string _imagePath;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            string rootServerPath = Directory.GetParent(HttpContext.Server.MapPath("//")).Parent.FullName;
            _imagePath = Path.Combine(rootServerPath, @"Images\MapPlacedImages\");
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }


        }

        public PlacedItemController()
        {
            _eneoCtx = new EneoContext();
            _placedItemsRepo = new PlacedItemsRepo(_eneoCtx);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public HttpResponseMessage GetMapItemImage(string id)
        {
            HttpResponseMessage result = ImageHelper.GetImage(id, _imagePath);
            return result;
        }

        public ActionResult PlacedMapItems_Read([DataSourceRequest] DataSourceRequest request, PlacedItemModel model)
        {
            var items = _placedItemsRepo.GetMapItems();
            return Json(items.ToDataSourceResult(request));
        }

        public ActionResult PlacedItemComments_Read([DataSourceRequest] DataSourceRequest request, PlacedItemCommentModel model, int placedItemID)
        {
            var items = _placedItemsRepo.GetComments(placedItemID);
            return Json(items.ToDataSourceResult(request));
        }

        public ActionResult PlacedItemComments_Delete([DataSourceRequest] DataSourceRequest request, PlacedItemCommentModel model)
        {
            _placedItemsRepo.DeletePlacedItemComment(model.CommentID);
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AllowAnonymous]
        public ActionResult Test()
        {
            string path1 = HttpContext.Server.MapPath("~//App_Data//");
            string path2 = HttpContext.Server.MapPath("//App_Data//");
            string path3 = HttpContext.Server.MapPath("//");
            string path4 = HttpContext.Server.MapPath("~");
            string path5 = HttpContext.Server.MapPath("");

            return Json(new Tuple<string, string, string, string, string, string>(path1, path2, path3, path4, path5, ""), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {
            var mapPlacedItem = _placedItemsRepo.GetMapItem(id);
            ViewBag.CollectionItemList = GetCollectionItemSelectList;
            return View(mapPlacedItem);
        }

        [HttpPost]
        public ActionResult Edit(PlacedItemModel item, HttpPostedFileBase file)
        {
            if (file != null)
            {
                SaveImage(item, file);
            }
            if (ModelState.IsValid)
            {
                item = _placedItemsRepo.UpdateMapItem(item);
            }
            ViewBag.CollectionItemList = GetCollectionItemSelectList;
            return View(item);
        }

        private void SaveImage(PlacedItemModel item, HttpPostedFileBase file)
        {
            file.SaveAs(_imagePath + file.FileName);
            string fileGuid = Guid.NewGuid().ToString();
            System.IO.File.Move(_imagePath + file.FileName, _imagePath + fileGuid);
            item.ImageUrl = fileGuid;
        }

        public ActionResult GetWikiUrl()
        {
            string wikiUrl = "http://pl.wikipedia.org/wiki/Zabytki_Poznania";
            WikiUrlModel wiki = new WikiUrlModel
            {
                Value = wikiUrl
            };
            return View(wiki);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoadFromWiki(WikiUrlModel wikiUrl)
        {
            var list = GetWikiPlaces(wikiUrl.Value).ToList();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoadedFromWiki(IEnumerable<PlacedItemWiki> model)
        {
            var itemsToAdd = model.Where(u => u.Add == true);
            GetWikiImages(itemsToAdd);
            _placedItemsRepo.AddWikiPlaces(itemsToAdd);
            return RedirectToAction("Index");
        }

        private void GetWikiImages(IEnumerable<PlacedItemWiki> places)
        {
            Parallel.ForEach(places, place =>
                {
                    if (!string.IsNullOrEmpty(place.ImageUrl))
                    {
                        WebClient client = new WebClient();
                        client.DownloadFile(new Uri(place.ImageUrl), _imagePath + place.Name);
                        Guid imageGuid = Guid.NewGuid();
                        System.IO.File.Move(_imagePath + place.Name, _imagePath + imageGuid.ToString());
                        place.ImageUrl = imageGuid.ToString();
                    }
                });
        }



        public ActionResult Add()
        {
            ViewBag.CollectionItemList = GetCollectionItemSelectList;
            return View(new PlacedItemModel
            {
                Latitude = "52.406391",
                Longitude = "16.925341",
                AddedDate = DateTime.Now
            });
        }

        [HttpPost]
        public ActionResult Add(PlacedItemModel item, HttpPostedFileBase file)
        {
            if (file != null)
            {
                SaveImage(item, file);
            }
            if (ModelState.IsValid)
            {
                item = _placedItemsRepo.AddMapItem(item);
            }
            ViewBag.CollectionItemList = GetCollectionItemSelectList;
            return RedirectToAction("Edit", new { id = item.PlacedItemID });
        }

        public ActionResult PlacedMapItem_Delete([DataSourceRequest] DataSourceRequest request, PlacedItemModel model)
        {
            _placedItemsRepo.DeleteMapItem(model.PlacedItemID);
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PlacedMapItems_LoadFromWiki([DataSourceRequest] DataSourceRequest request, PlacedItemWiki model)
        {
            List<PlacedItemWiki> list = GetWikiPlaces("http://pl.wikipedia.org/wiki/Zabytki_Poznania");
            return Json(list.ToDataSourceResult(request));
        }

        private static List<PlacedItemWiki> GetWikiPlaces(string wikiUrl)
        {
            List<PlacedItemWiki> list = new List<PlacedItemWiki>();
            Uri uri = new Uri(wikiUrl);
            string wikipedia = uri.GetLeftPart(UriPartial.Authority);
            string name = wikiUrl.Replace(wikipedia, string.Empty);


            var dom = CQ.CreateFromUrl(wikiUrl);
            CQ hrefs = dom.Select("a").RemoveAttr("title").RemoveAttr("class").RemoveAttr("accesskey");

            Regex regexHref = new Regex(@"<a href=""(.*)"">");
            Regex regexGeo = new Regex("[A-Z](.*)[0-9][0-9]°");

            List<string> allLinks = new List<string>();

            foreach (var href in hrefs)
            {
                var v = regexHref.Match(href.ToString());
                string s = v.Groups[1].ToString();
                if (s.Contains(".jpg") == false && s.Contains(".JPG") == false && s.Contains(".svg") == false && s.Contains("wikimedia") == false
                    && s.Contains("Wikipedia") == false && s.Contains("wikidata") == false && s.Contains("Pomoc") == false && s.Contains(name) == false
                    && s.Contains("mediawiki") == false && s.Contains("Specjalna") == false && s.Contains("Portal") == false && s.Contains("/wiki/") == true)
                    allLinks.Add(wikipedia + s);
            }

            foreach (var link in allLinks)
            {
                PlacedItemWiki item = new PlacedItemWiki();

                CQ domInternal = null;
                try
                {
                    domInternal = CQ.CreateFromUrl(link);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                if (domInternal != null)
                {
                    CQ latitudes = domInternal.Select(".latitude").Text();
                    CQ longitudes = domInternal.Select(".longitude").Text();
                    CQ names = domInternal.Select("#firstHeading").Text();
                    CQ description = domInternal.Select("#mw-content-text").Select("p:first").Text();
                    CQ imageUrl = domInternal.Select("#mw-content-text img").Attr("src");

                    var vLatitude = regexGeo.Match(latitudes.ToString());
                    string sLatitude = vLatitude.Groups[1].ToString();
                    var vLongitude = regexGeo.Match(longitudes.ToString());
                    string sLongitude = vLongitude.Groups[1].ToString();

                    if (latitudes.ToString() != "")
                    {
                        item.Name = names.ToString().Replace("\n", string.Empty).Replace("\t", string.Empty);
                        item.Latitude = sLatitude.Replace(" ", string.Empty);
                        item.Longitude = sLongitude.Replace(" ", string.Empty);
                        item.ImageUrl = "http:"+imageUrl.Text();
                        item.Add = true;
                        item.Description = description.Text();
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private IEnumerable<SelectListItem> GetCollectionItemSelectList
        {
            get
            {
                IList<CollectionItem> collectionItems = _eneoCtx.CollectionItems.ToList();
                return collectionItems.OrderBy(ci => ci.Name)
                    .Select(c =>
                    new SelectListItem
                    {
                        Selected = true,
                        Text = c.Name,
                        Value = c.CollectionItemID.ToString()
                    });
            }
        }
    }
}