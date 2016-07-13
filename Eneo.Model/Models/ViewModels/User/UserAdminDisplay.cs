using System;
using System.ComponentModel.DataAnnotations;

namespace Eneo.Model.Models.ViewModels.User
{
    public class UserAdminDisplay
    {
        //IdentityUser
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Id { get; set; }

        public bool LockoutEnabled { get; set; }

        public string PasswordHash { get; set; }

        [Display(Name = "Login")]
        public string UserName { get; set; }

        //EneoUser
        public string EneoUserID { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? RegisterDate { get; set; }

        public int CollectedItemsCount { get; set; }

        public string AvatarLink { get; set; }
    }
}