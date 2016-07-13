using Eneo.Helpers;
using Eneo.Model;
using Eneo.Model.Models.ViewModels.User;
using Eneo.Model.Repository;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Eneo.Site.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private EneoContext _eneoCtx;
        private UserRepo _userRepo;
        private string _avatarPath;

        public UserController()
        {
            _eneoCtx = new EneoContext();
            _userRepo = new UserRepo(_eneoCtx);
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Avatar(string id)
        {
            var result = ImageHelper.GetImage2(id, _avatarPath);
            return File(result , "application/ocet-stream","avatar.jpg");
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            string rootServerPath = Directory.GetParent(HttpContext.Server.MapPath("//")).Parent.FullName;
            _avatarPath = Path.Combine(rootServerPath, @"Images/Avatars/");
            if (!Directory.Exists(_avatarPath))
            {
                Directory.CreateDirectory(_avatarPath);
            }
        }

        public ActionResult Users_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Json(_userRepo.GetUsers().ToDataSourceResult(request));
        }

        public ActionResult User_Update([DataSourceRequest] DataSourceRequest request, UserAdminDisplay model)
        {
            _userRepo.UpdateUser(model);
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        public HttpResponseMessage GetAvatar(string id)
        {
            string path = "";
            HttpResponseMessage result = ImageHelper.GetImage(id, _avatarPath);
            return result;
        }

        public ActionResult Edit(string id)
        {
            var user = _userRepo.GetUser(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult Edit(UserAdminDisplay user, HttpPostedFileBase file)
        {
            if (file != null)
            {
                file.SaveAs(_avatarPath + file.FileName);
                string fileGuid = Guid.NewGuid().ToString();
                System.IO.File.Move(_avatarPath + file.FileName, _avatarPath + fileGuid);
                user.AvatarLink = fileGuid;
            }
            if (ModelState.IsValid)
            {
                _userRepo.UpdateUser(user);
            }
            return RedirectToAction("Edit", new { id = user.Id });
        }
    }
}