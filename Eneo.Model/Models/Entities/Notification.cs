using System.ComponentModel.DataAnnotations;

namespace Eneo.Model.Models.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        public string EneoUserID { get; set; }

        public string Content { get; set; }

        public NotificationType NotificationType { get; set; }

        public virtual EneoUser EneoUser { get; set; }
    }

    public enum NotificationType
    {
        Message,
        Achievement,
    }
}