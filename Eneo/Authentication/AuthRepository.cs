using Eneo.Model;
using Eneo.Model.Models.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eneo.Authentication
{
    public class AuthRepository : IDisposable
    {
        private EneoContext _ctx;

        private UserManager<IdentityUser> _userManager;

        public AuthRepository()
        {
            _ctx = new EneoContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(UserLogin userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            if (result.Succeeded)
                AddEneoUser(user);
            return result;
        }

        private void AddEneoUser(IdentityUser userModel)
        {
            EneoUser eneoUser = GetEneoUser(userModel);
            _ctx.EneoUsers.Add(eneoUser);
            _ctx.SaveChanges();
        }

        private static EneoUser GetEneoUser(IdentityUser userModel)
        {
            EneoUser eneoUser = new EneoUser
            {
                EneoUserID = userModel.Id,
                Name = userModel.UserName,
                DisplayName = userModel.UserName,
                RegisterDate = DateTime.Now
            };
            return eneoUser;
        }

        public void AddEneoUserByExternalProvider(IdentityUser userModel, string provider, string userIDFromProvider)
        {
            EneoUser eneoUser = GetEneoUser(userModel);
            eneoUser.Provider = provider;
            eneoUser.UserIDFromProvider = userIDFromProvider;
            eneoUser.Name = userModel.UserName;
            _ctx.EneoUsers.Add(eneoUser);
            _ctx.SaveChangesAsync();
        }

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);
            return user;
        }

        public IdentityUser FindUserByNameAndId(string userName, string userIDFromProvider)
        {
            IdentityUser user = _ctx.Users.SingleOrDefault(u => u.UserName == userName);
            var externalUser = _ctx.EneoUsers.SingleOrDefault(u => u.UserIDFromProvider == userIDFromProvider);
            if (externalUser == null)
                return null;

            return user;
        }

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _userManager.FindAsync(loginInfo);
            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingTokens = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId);
            if (existingTokens != null)
            {
                var result = await RemoveRefreshTokens(existingTokens);
            }
            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<bool> RemoveRefreshTokens(IEnumerable<RefreshToken> refreshTokens)
        {
            _ctx.RefreshTokens.RemoveRange(refreshTokens);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }
    }
}