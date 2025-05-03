using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Models;

namespace semsary_backend.Service
{
    public static class ApplicationDbSeeder
    {
        public static  void SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApiContext>();

            string adminEmail = "semsary.app@gmail.com";
            string adminPassword = "Recoloring"; // Choose a strong password
            
            // Check if admin user exists
            var adminUser =  dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m=>m.email==adminEmail));


            if (adminUser == null)
            {
                // Hash the password (don't store plain text)
                var passwordHash = PasswordHelper.HashPassword(adminPassword);

                var email = new Email
                {
                    email = adminEmail,
                    IsVerified = true,


                };

                var adminUser2 = new Admin
                {
                    Firstname="semsary",
                    Lastname="app",
                    password = passwordHash,
                    UserType=Enums.UserType.Admin,

                };
                adminUser2.Emails = new List<Email> { email };
                email.owner= adminUser2;
                email.ownerUsername= adminUser2.Username;

                dbContext.Admin.Add(adminUser2);
                 dbContext.SaveChanges();

            }
            else
            {

            }
        }
    }

}
