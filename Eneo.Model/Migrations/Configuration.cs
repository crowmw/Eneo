namespace Eneo.Model.Migrations
{
    using Eneo.Model.Models.Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<Eneo.Model.EneoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Eneo.Model.EneoContext";
        }

        protected override void Seed(Eneo.Model.EneoContext context)
        {
            context.Roles.AddOrUpdate(
                r => r.Name,
                new IdentityRole("Admin"));

            ApplicationUser admin = new ApplicationUser
                {
                    UserName = "Admin",
                    PasswordHash = "ACzFB/YbbRDHH1n/BHzZkX57JTEGIkkr8ma3zm4KsYL3gDYgKHSMuMl3hCWFNZyRkg==",
                    SecurityStamp = "c7f6b590-df8f-45bc-9fdd-68725d338569",
                    Email = "admin@example.com"
                };

            context.Users.AddOrUpdate(
                u => u.UserName,
                admin
                );


            context.Clients.AddOrUpdate(
                c => c.Id,
                new Client
                {
                    Active = true,
                    ApplicationType = ApplicationTypes.JavaScript,
                    Id = "WP",
                    Secret = "wpSecret!",
                    RefreshTokenLifeTime = 7200,
                    Name = "Windows Phone"
                });
        }
    }
}
