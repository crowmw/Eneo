using System;

namespace Eneo.Model.Models.ViewModels.User
{
    public class UserProfile
    {
        public string UserID { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? RegisterDate { get; set; }

        public int CollectedItemsCount { get; set; }

        public string AvatarLink { get; set; }
    }
}