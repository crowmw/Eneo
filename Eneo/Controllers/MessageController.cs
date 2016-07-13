using Eneo.Model;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Repository;
using System.Linq;
using System.Web.Http;

namespace Eneo.Controllers
{
    [Authorize]
    public class MessageController : ApiController
    {
        private EneoContext _eneoCtx;
        private MessageRepo _messageRepo;
        private UserRepo _userRepo;

        public MessageController()
        {
            _eneoCtx = new EneoContext();
            _messageRepo = new MessageRepo(_eneoCtx);
            _userRepo = new UserRepo(_eneoCtx);
        }

        public IQueryable<MessageModel> GetUnreaded()
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            return _messageRepo.GetUnreadedMessages(userID);
        }

        public IQueryable<MessageModel> GetAll()
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            return _messageRepo.GetAllMessages(userID);
        }

        [HttpPost]
        public IHttpActionResult SendMessage(MessageSendModel model)
        {
            string userID = _userRepo.GetUserID(User.Identity.Name);
            _messageRepo.SendMessage(userID, model.ReceiverUserID, model.Content);
            return Ok("Message send");
        }
    }
}