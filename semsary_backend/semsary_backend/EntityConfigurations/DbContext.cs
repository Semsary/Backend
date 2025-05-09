using Microsoft.EntityFrameworkCore;
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

            //modelBuilder.Entity<Email>()
            //.HasOne(e => e.owner)
            //.WithMany(u => u.Emails)
            //.HasForeignKey(e => e.ownerUsername);

            //modelBuilder.Entity<SermsaryUser>().HasData(
            //    new SermsaryUser { Username = "basant", Firstname = "b", Lastname = "b" , password = "jhg765*&JHy" , UserType = Enums.UserType.Customerservice }
            //    );

            //modelBuilder.Entity<Email>().HasData(
            //    new Email {email = "basant@gmail.com" , IsVerified=true , ownerUsername="basant"}
            //    ); 


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


    }
}
