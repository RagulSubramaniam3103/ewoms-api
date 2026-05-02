using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StartUpCompany.MainModel.Data_Admin_Staff;
using StartUpCompany.MainModel.Data_Student;

namespace StartUpCompany.MainModel.Data_DB
{
    public class DataDBContext : IdentityDbContext<MasterUsers>
    {
        public DataDBContext(DbContextOptions dbContext) : base(dbContext)
        {
        }
        public DbSet<MasterStudent> MasterUsers { get; set; }
        public DbSet<MasterStaff> MasterStaff { get; set; }
        public DbSet<MasterAdmin> MasterAdmin { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity table names

            builder.Entity<MasterUsers>().ToTable("StudentIdentityUsers");
            builder.Entity<IdentityRole>().ToTable("StudentIdentityRoles");
            builder.Entity<IdentityUserRole<string>>().ToTable("StudentIdentityUserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("StudentIdentityUserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("StudentIdentityUserLogins");
            builder.Entity<IdentityUserToken<string>>().ToTable("StudentIdentityUserTokens");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("StudentIdentityRoleClaims");


            // Fluent API for one-to-one relationship between MasterUsers and MasterStudent/MasterStaff

            builder.Entity<MasterAdmin>().HasOne(s => s.User).WithOne(u => u.IsAdmin).HasForeignKey<MasterAdmin>(s => s.UserId);
            builder.Entity<MasterStudent>().HasOne(s => s.User).WithOne(u => u.IsStudent).HasForeignKey<MasterStudent>(s => s.UserId);
            builder.Entity<MasterStaff>().HasOne(s => s.User).WithOne(u => u.IsStaff).HasForeignKey<MasterStaff>(s => s.UserId);


        }
    }
}
