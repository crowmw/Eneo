using Eneo.Model.Models.Entities;
using Eneo.Model.Models.SerializeModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Eneo.Model.Repository
{
    public class ExportRepo : EneoRepository
    {
        public ExportRepo(EneoContext context)
            : base(context)
        {
        }

        public List<EneoUser> GetEneoUsers()
        {
            var users = _eneoCtx.EneoUsers.ToList().Select(n => new EneoUser
            {
                AvatarGuid = n.AvatarGuid,
                CollectedItems = n.CollectedItems,
                DisplayName = n.DisplayName,
                DMPFacebook = n.DMPFacebook,
                EneoUserID = n.EneoUserID,
                LastLatitude = n.LastLatitude,
                LastLoginDate = n.LastLoginDate,
                LastLongitude = n.LastLongitude,
                Name = n.Name,
                PlacedItemComments = n.PlacedItemComments,
                Provider = n.Provider,
                RegisterDate = n.RegisterDate,
                UserIDFromProvider = n.UserIDFromProvider
            }).ToList();

            return users;
        }

        public List<CollectionItem> GetCollectionItems()
        {
            var items = _eneoCtx.CollectionItems.ToList().Select(n => new CollectionItem
                {
                    AddedDate = n.AddedDate,
                    CollectionItemID = n.CollectionItemID,
                    Name = n.Name
                }).ToList();
            return items;
        }

        public List<DMPDeviceInfo> GetDMPDeviceInfo()
        {
            var infos = _eneoCtx.DMPDeviceInfos.ToList().Select(n => new DMPDeviceInfo
                {
                    DMPDeviceInfoID = n.DMPDeviceInfoID,
                    EneoUserID = n.EneoUserID,
                    OperatingSystem = n.OperatingSystem,
                    SystemFirmwareVersion = n.SystemFirmwareVersion,
                    SystemHardwareVersion = n.SystemHardwareVersion,
                    SystemManufacturer = n.SystemManufacturer,
                    SystemProductName = n.SystemProductName
                }).ToList();

            return infos;
        }


        public List<DMPFacebook> GetDMPFacebook()
        {
            var fbData = _eneoCtx.DMPFacebook.Include(x => x.DMPFacebookLikes).ToList().Select(n => new DMPFacebook
                {
                    DMPFacebookID = n.DMPFacebookID,
                    Email = n.Email,
                    EneoUserID = n.EneoUserID,
                    FacebookID = n.FacebookID,
                    FirstName = n.FirstName,
                    Gender = n.Gender,
                    Language = n.Language,
                    LastName = n.LastName,
                    Locale = n.Locale,
                    Timezone = n.Timezone,
                    Verified = n.Verified,
                    DMPFacebookLikes = n.DMPFacebookLikes.ToList().Select(l => new DMPFacebookLikes
                    {
                        AddedDate = l.AddedDate,
                        CategoryName = l.CategoryName,
                        DMPFacebookID = l.DMPFacebookID,
                        DMPFacebookLikesID = l.DMPFacebookLikesID,
                        LikeID = l.LikeID,
                        LikeName = l.LikeName
                    }).ToList(),
                }).ToList();

            return fbData;
        }

        public List<DMPUserPosition> GetDMPUserPosition()
        {
            var positions = _eneoCtx.DMPUserPositions.ToList().Select(n => new DMPUserPosition
                {
                    Date = n.Date,
                    DMPUserPositionID = n.DMPUserPositionID,
                    EneoUserID = n.EneoUserID,
                    Latitude = n.Latitude,
                    Longitude = n.Longitude
                }).ToList();

            return positions;
        }

        public List<PlacedItem> GetPlacedItems()
        {
            var placedItems = _eneoCtx.PlacedItems.ToList().Select(n => new PlacedItem
                {
                    AddedDate = n.AddedDate,
                    CollectedItems = n.CollectedItems,
                    CollectionItem = n.CollectionItem,
                    CollectionItemID = n.CollectionItemID,
                    Description = n.Description,
                    ImageGuid = n.ImageGuid,
                    Latitude = n.Latitude,
                    Longitude = n.Longitude,
                    PlacedItemID = n.PlacedItemID,
                    Stars = n.Stars,
                }).ToList();

            return placedItems;
        }

        public void AddPlacedItems(IEnumerable<PlacedItem> places)
        {
            _eneoCtx.PlacedItems.AddRange(places);
            _eneoCtx.SaveChanges();
        }

        public void AddAspNetUsers(IEnumerable<IdentityUserSM> users)
        {
            foreach (var user in users)
            {
                _eneoCtx.Users.Add(new ApplicationUser
                    {
                        AccessFailedCount = user.AccessFailedCount,
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,
                        Id = user.Id,
                        LockoutEnabled = user.LockoutEnabled,
                        LockoutEndDateUtc = user.LockoutEndDateUtc,
                        PasswordHash = user.PasswordHash,
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                        SecurityStamp = user.SecurityStamp,
                        TwoFactorEnabled = user.TwoFactorEnabled,
                        UserName = user.UserName
                    });
                try
                {
                    //_eneoCtx.Database.SqlQuery(typeof(int), "update AspNetUsers set Discriminator='ApplicationUser'",null);
                    _eneoCtx.SaveChanges();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public void AddEneoUsers(IEnumerable<EneoUser> users)
        {
            _eneoCtx.EneoUsers.AddRange(users);
            _eneoCtx.SaveChanges();

        }

        public List<IdentityUserSM> GetAspNetUsers()
        {
            var users = _eneoCtx.Users.ToList().Select(n => new IdentityUserSM
                {
                    AccessFailedCount = n.AccessFailedCount,
                    //Discrimnator = "ApplicationUser",
                    Email = n.Email,
                    EmailConfirmed = n.EmailConfirmed,
                    Id = n.Id,
                    LockoutEnabled = n.LockoutEnabled,
                    LockoutEndDateUtc = n.LockoutEndDateUtc,
                    PasswordHash = n.PasswordHash,
                    PhoneNumber = n.PhoneNumber,
                    PhoneNumberConfirmed = n.PhoneNumberConfirmed,
                    SecurityStamp = n.SecurityStamp,
                    TwoFactorEnabled = n.TwoFactorEnabled,
                    UserName = n.UserName
                }).ToList();

            return users;
        }
    }
}
