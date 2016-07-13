﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eneo.Model.Models.SerializeModels
{
    public class IdentityUserSM
    {
        public int AccessFailedCount { get; set; }
        public string Email { get; set; }

        public string Discrimnator { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string SecurityStamp { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; }
    }
}
