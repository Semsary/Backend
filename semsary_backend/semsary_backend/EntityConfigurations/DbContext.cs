using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using semsary_backend.Models;

namespace semsary_backend.EntityConfigurations
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApiContext).Assembly);

        }


        public DbSet<Models.SermsaryUser> SermsaryUsers { get; set; }
        public DbSet<Models.Landlord> Landlords { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<CustomerService> CustomerServices { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<UnverifiedUser>UnverifiedUsers { get; set; }
        public DbSet<IdentityDocument>identityDocuments { get; set; }

        public DbSet<House> Houses { get; set; }
        public DbSet<HouseInspection> HouseInspections { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalUnit> RentalUnits { get; set; }

    }
}
