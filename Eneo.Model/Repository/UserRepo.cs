using Eneo.Model.Models;
using Eneo.Model.Models.Entities;
using Eneo.Model.Models.ViewModels;
using Eneo.Model.Models.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using Eneo.Helpers;

namespace Eneo.Model.Repository
{
    public class UserRepo : EneoRepository
    {
        private string _appDataPath, _avatarPath;

        public UserRepo(EneoContext context)
            : base(context)
        {
        }

        public string GetUserID(string userName)
        {
            return GetEneoUser(userName).EneoUserID;
        }

        public void SaveAvatar(Guid avatarGuid, string userID)
        {
        }

        public EneoUser GetEneoUser(string name)
        {
            return _eneoCtx.EneoUsers.Single(u => u.Name == name);
        }

        public EneoUser GetEneoUserByID(string userID)
        {
            return _eneoCtx.EneoUsers.Single(u => u.EneoUserID == userID);
        }

        public void UpdatePosition(string userID, LastPosition position)
        {
            var user = _eneoCtx.EneoUsers.Single(u => u.EneoUserID == userID);
            user.LastLatitude = position.Latitude;
            user.LastLongitude = position.Longitude;

            _eneoCtx.SaveChanges();
        }

        public void UpdateAvatar(string username, Guid avatarGuid)
        {
            var user = _eneoCtx.EneoUsers.Single(u => u.Name == username);
            user.AvatarGuid = avatarGuid;
            _eneoCtx.SaveChanges();
        }

        #region UserProfile

        public UserProfile GetUserProfile(string userID)
        {
            var user = _eneoCtx.Users.Join(_eneoCtx.EneoUsers,
                 us => us.Id,
                 eu => eu.EneoUserID,
                 (us, eu) => new UserProfile
                 {
                     Email = us.Email,
                     LastLoginDate = eu.LastLoginDate,
                     RegisterDate = eu.RegisterDate,
                     CollectedItemsCount = eu.CollectedItems.Count,
                     UserName = eu.Name,
                     UserID = us.Id,
                     AvatarLink = eu.AvatarGuid.HasValue ? "http://eneo.azurewebsites.net/api/User/Avatar/" + eu.AvatarGuid : string.Empty
                 }
                 ).Single(u => u.UserID == userID);

            return user;
        }

        #endregion UserProfile

        #region UserRanking

        public List<UserRanking> GetUsersRanking()
        {
            var users = _eneoCtx.Users.Join(_eneoCtx.EneoUsers,
                 us => us.Id,
                 eu => eu.EneoUserID,
                 (us, eu) => new UserRanking
                 {
                     UserID = us.Id,
                     UserName = eu.Name,
                     CollectedItemsCount = eu.CollectedItems.Count,
                 }).ToList();

            return users;
        }

        #endregion UserRanking

        #region UserAdminDisplay

        public IQueryable<UserAdminDisplay> GetUsers()
        {
            var users = _eneoCtx.Users.Join(_eneoCtx.EneoUsers,
                us => us.Id,
                eu => eu.EneoUserID,
                (us, eu) => new UserAdminDisplay
                {
                    Email = us.Email,
                    EmailConfirmed = us.EmailConfirmed,
                    Id = us.Id,
                    EneoUserID = eu.EneoUserID,
                    LockoutEnabled = us.LockoutEnabled,
                    UserName = us.UserName,
                    PasswordHash = us.PasswordHash,
                    RegisterDate = eu.RegisterDate,
                    LastLoginDate = eu.LastLoginDate,
                    AvatarLink = "http://eneo.azurewebsites.net/User/Avatar/" + eu.AvatarGuid,
                    CollectedItemsCount = eu.CollectedItems.Count
                });

            return users;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance">Distance in kms</param>
        /// <returns></returns>
        public IOrderedEnumerable<NearestUser> GetNearestUsers(string userName, double userLatitude, double userLongitude, double distance)
        {

            var users = _eneoCtx.Users.Join(_eneoCtx.EneoUsers,
               us => us.Id,
               eu => eu.EneoUserID,
               (us, eu) => new NearestUser
               {
                   LastLatitude = eu.LastLatitude,
                   LastLongitude = eu.LastLongitude,
                   Distance = 0.0,
                   UserID = eu.EneoUserID,
                   UserName = eu.Name
               }).Where(u => u.UserName != userName).ToList();


            foreach (var user in users)
            {
                user.Distance = Math.Round(DistanceHelper.CalculateDistance(userLatitude, userLongitude, user.LastLatitude, user.LastLongitude), 1);
            }
            return users.OrderBy(d => d.Distance);
        }

        public UserAdminDisplay GetUser(string id)
        {
            var user = _eneoCtx.Users.Join(_eneoCtx.EneoUsers,
                us => us.Id,
                eu => eu.EneoUserID,
                (us, eu) => new UserAdminDisplay
                {
                    Email = us.Email,
                    EmailConfirmed = us.EmailConfirmed,
                    Id = us.Id,
                    EneoUserID = eu.EneoUserID,
                    LockoutEnabled = us.LockoutEnabled,
                    UserName = us.UserName,
                    PasswordHash = us.PasswordHash,
                    RegisterDate = eu.RegisterDate,
                    AvatarLink = string.IsNullOrEmpty(eu.AvatarGuid.ToString()) ? string.Empty : "http://eneo.azurewebsites.net/User/Avatar/" + eu.AvatarGuid.ToString(),
                    LastLoginDate = eu.LastLoginDate,
                    CollectedItemsCount = eu.CollectedItems.Count
                }
                ).Single(u => u.Id == id);

            return user;
        }

        public void UpdateUser(UserAdminDisplay user)
        {
            UpdateIdentityUser(user);
            UpdateEneoUser(user);
            _eneoCtx.SaveChanges();
        }

        private void UpdateIdentityUser(UserAdminDisplay user)
        {
            var identityUser = _eneoCtx.Users.Single(us => us.Id == user.Id);
            identityUser.Email = user.Email;
            identityUser.EmailConfirmed = user.EmailConfirmed;
            identityUser.LockoutEnabled = user.LockoutEnabled;
            identityUser.UserName = user.UserName;
        }

        private void UpdateEneoUser(UserAdminDisplay user)
        {
            var eneoUser = _eneoCtx.EneoUsers.Single(us => us.EneoUserID == user.Id);
            //eneoUser.LastLoginDate = user.LastLoginDate;
            //eneoUser.RegisterDate = user.RegisterDate;
            eneoUser.Name = user.UserName;
            eneoUser.AvatarGuid = new Guid(user.AvatarLink);
        }

        #endregion UserAdminDisplay
    }
}