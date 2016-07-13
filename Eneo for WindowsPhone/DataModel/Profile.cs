using System;

namespace Eneo.WindowsPhone.DataModel
{
    public class Profile
    {
        public string UserID { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public DateTime LastLoginDate { get; set; }

        public DateTime RegisterDate { get; set; }

        public int CollectedItemsCount { get; set; }

        public string AvatarLink { get; set; }
    }
}