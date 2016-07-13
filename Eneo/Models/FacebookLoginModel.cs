using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eneo.Models
{
    public class FacebookLoginModel
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
    }

    public class FacebookUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}