namespace Eneo.Site.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    //public class ApplicationUser : IdentityUser
    //{
    //    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
    //    {
    //        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
    //        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
    //        // Add custom user claims here
    //        return userIdentity;
    //    }
    //}

    //public class EneoContext : IdentityDbContext<ApplicationUser>
    //{
    //    public EneoContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    static EneoContext()
    //    {
    //        // Set the database intializer which is run once during application start
    //        // This seeds the database with admin user credentials and admin role
    //        Database.SetInitializer<EneoContext>(new ApplicationDbInitializer());
    //    }

    //    public static EneoContext Create()
    //    {
    //        return new EneoContext();
    //    }
    //}
}