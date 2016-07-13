using Eneo.Model;
using Eneo.Model.Models.ViewModels.DMP;
using Eneo.Model.Repository;
using System.Web.Http;

namespace Eneo.Controllers
{
    [Authorize]
    public class DMPController : ApiController
    {
        private EneoContext _eneoCtx;
        private DMPRepo _dmpRepo;
        private UserRepo _userRepo;

        public DMPController()
        {
            _eneoCtx = new EneoContext();
            _dmpRepo = new DMPRepo(_eneoCtx);
            _userRepo = new UserRepo(_eneoCtx);
        }

        [HttpPost]
        public IHttpActionResult AddPosition(DMPPosition position)
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            _dmpRepo.AddPosition(position, userID);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult AddDevice(DMPDevice device)
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            _dmpRepo.AddDevice(device, userID);
            return Ok();
        }

        [HttpPost]
        public IHttpActionResult AddFacebook(DMPFacebookModel facebook)
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            _dmpRepo.AddFacebook(facebook, userID);
            return Ok();
        }
    }
}