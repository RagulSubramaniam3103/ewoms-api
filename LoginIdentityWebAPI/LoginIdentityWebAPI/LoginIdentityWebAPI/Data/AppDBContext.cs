using LoginIdentityWebAPI.FlightModels;
using LoginIdentityWebAPI.UserControlled;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoginIdentityWebAPI.Data
{
    public class AppDBContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDBContext(DbContextOptions options) : base(options) { }

        #region test
        public DbSet<UserMainDetails> UserMainDetails { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);


        //    builder.Entity<ApplicationUser>(b => b.ToTable("TestUsers"));
        //    builder.Entity<IdentityRole<int>>(b => b.ToTable("TestRoles"));
        //    builder.Entity<IdentityUserRole<int>>(b => b.ToTable("TestUserRoles"));
        //    builder.Entity<IdentityUserClaim<int>>(b => b.ToTable("TestUserClaims"));
        //    builder.Entity<IdentityUserLogin<int>>(b => b.ToTable("TestUserLogins"));
        //    builder.Entity<IdentityRoleClaim<int>>(b => b.ToTable("TestRoleClaims"));
        //    builder.Entity<IdentityUserToken<int>>(b => b.ToTable("TestUserTokens"));

        //    // 1: ApplicationUser → UserMainDetails (1:1)
        //    builder.Entity<ApplicationUser>()
        //        .HasOne(u => u.UserMainDetails)
        //        .WithOne(um => um.ApplicationUser)
        //        .HasForeignKey<UserMainDetails>(um => um.ApplicationUserId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // 2: UserMainDetails → UserDetails (1:1)
        //    builder.Entity<UserMainDetails>()
        //        .HasOne(um => um.UserDetails)
        //        .WithOne(ud => ud.UserMainDetails)
        //        .HasForeignKey<UserDetails>(ud => ud.EmpId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // Optional: set decimal precision for salary
        //    builder.Entity<UserDetails>()
        //        .Property(ud => ud.EmpSalary)
        //        .HasColumnType("decimal(18,2)");
        //}

        #endregion

        public DbSet<ListFlight> FlightDetails { get; set; }
        public DbSet<AirportDetails> AirportDetails { get; set; }
        public DbSet<FlightTravelDetails> FlightTravelDetails { get; set; }
        public DbSet<FlightseatDetails> flightseatDetails { get; set; }
        public DbSet<FlightSeatPrice> FlightSeatPrice { get; set; }
        public DbSet<TimeZoneairport> timeZoneairports { get; set; }

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
