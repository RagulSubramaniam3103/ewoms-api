using FirstProjectWebAPI.MainData.ModelsMigration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstProjectWebAPI.MainData.DBContextFiles
{
    public class MainDBContext:IdentityDbContext
    {
        public MainDBContext(DbContextOptions<MainDBContext> options):base(options) { }
        public DbSet<CustomerAddress> MainCustomer_Address { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CustomerDetails>().ToTable("UsersDetailsUsers");
            builder.Entity<IdentityRole>().ToTable("UsersDetailsRoles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UsersDetailsUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UsersDetailsUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UsersDetailsUserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("UsersDetailsRoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UsersDetailsUserTokens");
        }

    }
}
