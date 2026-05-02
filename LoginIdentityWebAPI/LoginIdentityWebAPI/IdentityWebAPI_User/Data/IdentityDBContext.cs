using IdentityWebAPI_User.MainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityWebAPI_User.Data
{
    public class IdentityDBContext : IdentityDbContext<CustomerDetails>
    {
        public IdentityDBContext(DbContextOptions<IdentityDBContext> options):base(options)
        {

        }
        public DbSet<CustomerDetails> MasterCustomerDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<CustomerDetails>().ToTable("MasterCustomerDetails");
            builder.Entity<IdentityRole>().ToTable("MasterCustomerRoles");
            builder.Entity<IdentityUserRole<string>>().ToTable("CustomerRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("CustomerUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("CustomerLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("CustomerRolesClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("CustomerTokens");

        }
    }
}
