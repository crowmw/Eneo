using Eneo.Model.Models.Entities;
using Eneo.Model.Models.ViewModels.DMP;
using System;
using System.Linq;

namespace Eneo.Model.Repository
{
    public class DMPRepo : EneoRepository
    {
        public DMPRepo(EneoContext context)
            : base(context)
        {
            _eneoCtx = context;
        }

        public void AddPosition(DMPPosition position, string userID)
        {
            DMPUserPosition userPosition = new DMPUserPosition
            {
                Date = DateTime.Now,
                EneoUserID = userID,
                Latitude = position.Latitude,
                Longitude = position.Longitude
            };

            _eneoCtx.DMPUserPositions.Add(userPosition);
            _eneoCtx.SaveChangesAsync();
        }

        public IQueryable<DMPPositionSite> GetPositions(string userID)
        {

            return _eneoCtx.DMPUserPositions.Where(p => p.EneoUserID == userID)
            .Select(x => new DMPPositionSite
                {
                    Date = x.Date,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    UserName = x.EneoUser.Name
                });
        }

        public IQueryable<DMPDevice> GetDevice(string userID)
        {
            return _eneoCtx.DMPDeviceInfos.Where(p => p.EneoUserID == userID)
                .Select(x => new DMPDevice
                {
                    OperatingSystem = x.OperatingSystem,
                    SystemFirmwareVersion = x.SystemFirmwareVersion,
                    SystemHardwareVersion = x.SystemHardwareVersion,
                    SystemManufacturer = x.SystemManufacturer,
                    SystemProductName = x.SystemProductName
                });
        }

        public IQueryable<DMPFacebookModel> GetFacebook(string userID)
        {
            return _eneoCtx.DMPFacebook.Where(p => p.EneoUserID == userID)
                .Select(x => new DMPFacebookModel
                {
                    EneoUserID = x.EneoUserID,
                    FacebookID = x.FacebookID.ToString(),
                    Email = x.Email,
                    Gender = x.Gender,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Language = x.Language,
                    Locale = x.Locale,
                    Timezone = x.Timezone.ToString(),
                    Verified = x.Verified.ToString(),
                    DMPFacebookLikes = x.DMPFacebookLikes.Select(
                        l => new Eneo.Model.Models.ViewModels.DMP.DMPFacebookLikes
                        {
                            CategoryName = l.CategoryName,
                            LikeName = l.LikeName,
                            LikeID = l.LikeID,
                            AddedDate = l.AddedDate
                        }).ToList(),
                });
        }

        public void AddDevice(DMPDevice device, string userID)
        {
            DMPDeviceInfo deviceInfo = new DMPDeviceInfo
            {
                EneoUserID = userID,
                OperatingSystem = device.OperatingSystem,
                SystemFirmwareVersion = device.SystemFirmwareVersion,
                SystemHardwareVersion = device.SystemHardwareVersion,
                SystemManufacturer = device.SystemManufacturer,
                SystemProductName = device.SystemManufacturer
            };

            _eneoCtx.DMPDeviceInfos.Add(deviceInfo);
            _eneoCtx.SaveChangesAsync();
        }

        public void AddFacebook(DMPFacebookModel facebook, string userID)
        {
            Eneo.Model.Models.Entities.DMPFacebook facebookInfo = new Eneo.Model.Models.Entities.DMPFacebook
            {
                EneoUserID = userID,
                FacebookID = long.Parse(facebook.FacebookID),
                Email = facebook.Email,
                Gender = facebook.Gender,
                FirstName = facebook.FirstName,
                LastName = facebook.LastName,
                Language = facebook.Language,
                Locale = facebook.Locale,
                Timezone = Convert.ToInt32(facebook.Timezone),
                Verified = Convert.ToBoolean(facebook.Verified),
                DMPFacebookLikes = facebook.DMPFacebookLikes.Select(
                x => new Eneo.Model.Models.Entities.DMPFacebookLikes
                {
                    CategoryName = x.CategoryName,
                    LikeName = x.LikeName,
                    LikeID = x.LikeID,
                    AddedDate = x.AddedDate
                }).ToList(),
            };

            _eneoCtx.DMPFacebook.Add(facebookInfo);
            _eneoCtx.SaveChanges();
        }
    }
}