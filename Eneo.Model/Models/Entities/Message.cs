using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eneo.Model.Models.Entities
{
    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        public string SenderUserID { get; set; }

        public string ReceiverUserID { get; set; }

        public string Content { get; set; }

        public DateTime SendDate { get; set; }

        public bool Readed { get; set; }

        [ForeignKey("SenderUserID")]
        public virtual EneoUser SenderUser { get; set; }

        [ForeignKey("ReceiverUserID")]
        public virtual EneoUser ReceiverUser { get; set; }
    }
}