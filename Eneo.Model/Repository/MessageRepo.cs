using Eneo.Model.Models.Entities;
using Eneo.Model.Models.ViewModels;
using System;
using System.Linq;

namespace Eneo.Model.Repository
{
    public class MessageRepo : EneoRepository
    {
        public MessageRepo(EneoContext ctx)
            : base(ctx)
        {
        }

        public Message SendMessage(string senderUserID, string receiverUserID, string content)
        {
            Message message = new Message
            {
                Content = content,
                Readed = false,
                ReceiverUserID = receiverUserID,
                SenderUserID = senderUserID,
                SendDate = DateTime.Now
            };

            _eneoCtx.Messages.Add(message);
            _eneoCtx.SaveChangesAsync();

            return message;
        }

        public IQueryable<MessageModel> GetUnreadedMessages(string userID)
        {
            var messages = _eneoCtx.Messages.Where(m => m.ReceiverUserID == userID && m.Readed == false)
                .Select(n => new MessageModel
                {
                    Content = n.Content,
                    MessageID = n.MessageID,
                    Readed = n.Readed,
                    ReceiverUserName = n.ReceiverUser.Name,
                    SenderUserName = n.SenderUser.Name,
                    SendDate = n.SendDate
                });

            return messages;
        }

        public IQueryable<MessageModel> GetAllMessages(string userID)
        {
            var messages = _eneoCtx.Messages.Where(m => m.ReceiverUserID == userID)
                .Select(n => new MessageModel
                {
                    Content = n.Content,
                    MessageID = n.MessageID,
                    Readed = n.Readed,
                    ReceiverUserName = n.ReceiverUser.Name,
                    SenderUserName = n.SenderUser.Name,
                    SendDate = n.SendDate
                });
            return messages;
        }
    }
}