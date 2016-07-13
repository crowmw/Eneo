using Eneo.Model.Models.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Eneo.Model
{
    public class EneoContext : IdentityDbContext<IdentityUser>
    {
        public EneoContext()
            : base("EneoDB")
        {
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<EneoUser> EneoUsers { get; set; }

        public DbSet<PlacedItemComment> PlacedItemComments { get; set; }

        public DbSet<PlacedItem> PlacedItems { get; set; }

        public DbSet<CollectionItem> CollectionItems { get; set; }

        public DbSet<CollectedItem> CollectedItems { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<DMPUserPosition> DMPUserPositions { get; set; }

        public DbSet<DMPDeviceInfo> DMPDeviceInfos { get; set; }

        public DbSet<DMPFacebook> DMPFacebook { get; set; }

        public DbSet<DMPFacebookLikes> DMPFacebookLikes { get; set; }

        public static EneoContext Create()
        {
            return new EneoContext();
        }

    }
}