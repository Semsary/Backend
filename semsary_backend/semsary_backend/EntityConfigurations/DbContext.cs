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

            modelBuilder.Entity<SermsaryUser>()
                .OwnsOne(u => u.Address);

            modelBuilder.Entity<Coupon>()
                .Property(c => c.Id)
                .HasConversion(
                    ulid => ulid.ToString(),          // Convert Ulid to string for DB
                    str => Ulid.Parse(str));

            modelBuilder.Entity<BlockedId>()
                .HasOne(b => b.Landlord)
                .WithOne(l => l.BlockedId)
                .HasForeignKey<BlockedId>(b => b.LandlordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BlockedId>()
                .HasOne(b => b.customerService)
                .WithMany(c => c.BlockedIds)
                .HasForeignKey(b => b.BlockedBy);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.House)
                .WithMany(h => h.Comments)
                .HasForeignKey(c => c.HouseId)
                .OnDelete(DeleteBehavior.Cascade); // keep cascade here if you want house deletion to remove comments

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.TenantUsername)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rate>()
                .HasOne(r => r.House)
                .WithMany(h => h.Rates)
                .HasForeignKey(r => r.HouseId)
                .OnDelete(DeleteBehavior.Cascade); // allow deleting house to delete related rates

            modelBuilder.Entity<Rate>()
                .HasOne(r => r.Tenant)
                .WithMany()
                .HasForeignKey(r => r.TenantUsername)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.House)
                .WithMany(h => h.Rentals)
                .HasForeignKey(r => r.HouseId)
                .OnDelete(DeleteBehavior.Cascade); // keep if house deletion should remove rentals

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Tenant)
                .WithMany()
                .HasForeignKey(r => r.TenantUsername)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Rental)
                .WithOne(r => r.Complaint)
                .HasForeignKey<Complaint>(c => c.RentalId)
                .OnDelete(DeleteBehavior.Cascade); // allow deleting rental to remove complaint

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.CustomerService)
                .WithMany(cs => cs.Complaints)
                .HasForeignKey(c => c.VerifiedBy)
                .OnDelete(DeleteBehavior.Restrict); // ❌ prevent cascade path issue

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Tenant)
                .WithMany()
                .HasForeignKey(c => c.SubmittedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasMany(r => r.RentalUnit)
                .WithMany(ru => ru.Rentals)
                .UsingEntity<Dictionary<string, object>>(
                    "RentalRentalUnit",
                    j => j
                        .HasOne<RentalUnit>()
                        .WithMany()
                        .HasForeignKey("RentalUnitId")
                        .OnDelete(DeleteBehavior.Cascade), // cascade OK on this side
                    j => j
                        .HasOne<Rental>()
                        .WithMany()
                        .HasForeignKey("RentalsRentalId")
                        .OnDelete(DeleteBehavior.Restrict), // ❌ restrict this side
                    j =>
                    {
                        j.HasKey("RentalUnitId", "RentalsRentalId");
                    });


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
        public DbSet<Rate> Rates { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<BlockedId> BlockedIds { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Coupon>Coupons { get; set; }

    }
}
