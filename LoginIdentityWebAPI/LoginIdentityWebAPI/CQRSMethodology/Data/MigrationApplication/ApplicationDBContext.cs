using LoginIdentityWebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace CQRSMethodology.Data.MigrationApplication
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options) { }
        public DbSet<CustomerDetails> CustomerDetails { get; set; }
        public DbSet<CustomerCartPurchased> CustomerCartPurchased { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CustomerCartPurchased>()
                .HasOne(c => c.CustomerDetails)
                .WithMany(d => d.customerCartPurchaseds)
                .HasForeignKey(c => c.Customer_Id);
        }
    }
}
