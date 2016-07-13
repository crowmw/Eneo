using Eneo.Helpers;
using Eneo.Model;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Repository;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Eneo.Controllers
{
    [Authorize]
    public class MapController : ApiController
    {
        private EneoContext _eneoCtx;
        private PlacedItemsRepo _placedItemsRepo;
        private CollectedItemsRepo _collectionItemsRepo;
        private UserRepo _userRepo;
        private string _imagePath;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            string rootServerPath = Directory.GetParent(HttpContext.Current.Server.MapPath("//")).Parent.FullName;
            _imagePath = Path.Combine(rootServerPath, @"Images/MapPlacedImages/");
            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
            }
        }

        public MapController()
        {
            _eneoCtx = new EneoContext();
            _placedItemsRepo = new PlacedItemsRepo(_eneoCtx);
            _collectionItemsRepo = new CollectedItemsRepo(_eneoCtx);
            _userRepo = new UserRepo(_eneoCtx);
        }

        //na razie wszystkie punkty
        [AllowAnonymous]
        public IHttpActionResult GetItems(string range = "")
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            return Ok(_placedItemsRepo.GetMapItems(userID, range));
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetImage(string id)
        {
            var result = ImageHelper.GetImage(id, _imagePath);
            return result;
        }

        [HttpPost]
        public IHttpActionResult Checkin([FromBody] string mapPlacedItemID)
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            int parsedMapPlaceID = int.Parse(mapPlacedItemID);

            if (!_collectionItemsRepo.IsChecked(parsedMapPlaceID, userID))
            {
                _collectionItemsRepo.AddCheckin(parsedMapPlaceID, userID);
                return Ok("Checkin ok");
            }

            return Ok("Already checked");
        }

        [HttpPost]
        public IHttpActionResult AddComment(PlacedItemCommentModel model)
        {
            model.UserID = _userRepo.GetUserID(User.Identity.Name);

            if (_placedItemsRepo.AddComment(model))
                return Ok("Comment added");
            else
                return Ok("Comment already added");
        }

        [HttpGet]
        public IQueryable<CollectedItemModel> GetCollectedItems(string userID)
        {
            return _collectionItemsRepo.GetUserCollectedItems(userID);

        }

        [HttpGet]
        public IQueryable<PlacedItemCommentModel> GetComments(int id)
        {
            return _placedItemsRepo.GetComments(id);
        }
    }
}