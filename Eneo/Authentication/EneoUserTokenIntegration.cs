using Eneo.Model;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Eneo.Authentication
{
    public class EneoUserTokenIntegration
    {
        private EneoContext _eneoCtx;

        public EneoUserTokenIntegration()
        {
            _eneoCtx = new EneoContext();
        }

        public void AddPropertiesToToken(OAuthTokenEndpointContext context)
        {
            string userName = context.Identity.Name;
            var user = _eneoCtx.EneoUsers.SingleOrDefault(u => u.Name == userName);

            context.AdditionalResponseParameters.Add("LastLoginDate", user.LastLoginDate.ToString());
            context.AdditionalResponseParameters.Add("EneoUserID", user.EneoUserID);
            context.AdditionalResponseParameters.Add("DisplayName", user.DisplayName);
            user.LastLoginDate = DateTime.Now;
            _eneoCtx.SaveChangesAsync();
        }

        public void AddPropertiesToToken(JObject token, string userName)
        {
            var user = _eneoCtx.EneoUsers.SingleOrDefault(u => u.Name == userName);
            token.Add("LastLoginDate", user.LastLoginDate.ToString());
            token.Add("EneoUserID", user.EneoUserID);
            token.Add("DisplayName", user.DisplayName);
            user.LastLoginDate = DateTime.Now;
            _eneoCtx.SaveChangesAsync();
        }
    }
}