using Eneo.Model;
using Eneo.Model.Repository;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;

namespace Eneo.Site.Controllers
{
    [Authorize]
    public class DMPController : Controller
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

        public ActionResult Index()
        {
            var users = _userRepo.GetUsers();
            return View(users);
        }

        // GET: DMP
        public ActionResult DMPPositions(string userID)
        {
            var dmpPositions = _dmpRepo.GetPositions(userID);
            return View(dmpPositions);
        }

        public ActionResult DMPDevice(string userID)
        {
            var dmpDevice = _dmpRepo.GetDevice(userID);
            return View(dmpDevice);
        }

        public ActionResult DMPFacebook(string userID)
        {
            var dmpFacebook = _dmpRepo.GetFacebook(userID);
            return View(dmpFacebook);
        }
    }
}