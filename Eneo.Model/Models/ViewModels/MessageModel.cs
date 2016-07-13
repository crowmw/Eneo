using System;

namespace Eneo.Model.Models.ViewModels
{
    public class MessageModel
    {
        public int MessageID { get; set; }

        public string Content { get; set; }

        public DateTime SendDate { get; set; }

        public bool Readed { get; set; }

        public string SenderUserName { get; set; }

        public string ReceiverUserName { get; set; }
    }
}