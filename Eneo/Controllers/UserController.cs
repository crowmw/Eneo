using Eneo.Helpers;
using Eneo.Model;
using Eneo.Model.Models;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Models.ViewModels.User;
using Eneo.Model.Repository;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Linq;
using System.Web.Http;
using System.Globalization;
using System.Web.Hosting;
using System;

namespace Eneo.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private EneoContext _eneoCtx;
        private UserRepo _userRepo;
        private string _avatarPath;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            string rootServerPath = Directory.GetParent(HttpContext.Current.Server.MapPath("//")).Parent.FullName;
            _avatarPath = Path.Combine(rootServerPath, @"Images/Avatars/");
            if (!Directory.Exists(_avatarPath))
            {
                Directory.CreateDirectory(_avatarPath);
            }

        }

        public UserController()
        {
            _eneoCtx = new EneoContext();
            _userRepo = new UserRepo(_eneoCtx);
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Avatar(string id)
        {
            var result = ImageHelper.GetImage(id, _avatarPath);
            return result;
        }

        //public async Task<HttpResponseMessage> UploadAvatar (string userID)
        //{
        //    var eneoUser = _userRepo.GetEneoUserByID(userID);
        //    Guid avatarGuid = Guid.NewGuid();
        //}

        [HttpGet]
        public IHttpActionResult AvatarFromUrl(string url)
        {
            WebClient client = new WebClient();
            var data = client.DownloadData(url);
            Guid avatarGuid = Guid.NewGuid();
            var fileStream = File.Create(_avatarPath + avatarGuid.ToString());
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();

            _userRepo.UpdateAvatar(User.Identity.Name, avatarGuid);
            return Ok();
        }

        public async Task<HttpResponseMessage> UploadAvatar()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = _avatarPath;
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    //file.SaveAs(_avatarPath + file.FileName);
                    string fileGuid = Guid.NewGuid().ToString();
                    System.IO.File.Move(_avatarPath + file.LocalFileName, _avatarPath + fileGuid);

                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        public UserProfile GetProfile()
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            var userProfile = _userRepo.GetUserProfile(userID);

            return userProfile;
        }

        public IHttpActionResult UpdatePosition(LastPosition position)
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            _userRepo.UpdatePosition(userID, position);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public IOrderedEnumerable<NearestUser> Nearest(string latitude, string longitude)
        {
            double userLatitude;
            double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out userLatitude);
            double userLongitude;
            double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out userLongitude);
            string userName = User.Identity.Name;
            var users = _userRepo.GetNearestUsers(userName, userLatitude, userLongitude, 50.0);

            return users;
        }

        public UserProfile GetProfile(string userID)
        {
            var userProfile = _userRepo.GetUserProfile(userID);
            return userProfile;
        }

        public List<UserRanking> GetRanking()
        {
            return _userRepo.GetUsersRanking();
        }
    }
}