using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Models;

namespace semsary_backend.Service
{
    public static class ApplicationDbSeeder
    {
        public static void SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApiContext>();

            string adminEmail = "semsary.app@gmail.com";
            string adminPassword = "Recoloring"; // Choose a strong password

            // Check if admin user exists
            var adminUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == adminEmail));

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
                    Firstname = "semsary",
                    Lastname = "app",
                    password = passwordHash,
                    UserType = Enums.UserType.Admin,

                };
                adminUser2.Emails = new List<Email> { email };
                email.owner = adminUser2;
                email.ownerUsername = adminUser2.Username;

                dbContext.Admin.Add(adminUser2);
                dbContext.SaveChanges();
            }

            string CustomerServiceEmail = "CustomerService@gmail.com";
            string CustomerServicePassword = "jhg765*&JHy";


            var CustomerServiceUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == CustomerServiceEmail));
            if (CustomerServiceUser == null)
            {
                // Hash the password (don't store plain text)
                var passwordHash = PasswordHelper.HashPassword(CustomerServicePassword);

                var email = new Email
                {
                    email = CustomerServiceEmail,
                    IsVerified = true,

                };

                var CustomerServiceUser2 = new CustomerService
                {
                    Username = "CustomerService1",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,

                };
                CustomerServiceUser2.Emails = new List<Email> { email };
                email.owner = CustomerServiceUser2;
                email.ownerUsername = CustomerServiceUser2.Username;

                dbContext.CustomerServices.Add(CustomerServiceUser2);
                dbContext.SaveChanges();
            }

            string LandlordEmail = "Landlord@gmail.com";
            string LandlordPassword = "jhg765*&JHy";

            var LandlordUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == LandlordEmail));

            if(LandlordUser == null)
            {
                var passwordHash = PasswordHelper.HashPassword(LandlordPassword);

                var email = new Email
                {
                    email = LandlordEmail,
                    IsVerified = true
                };
                var LandlordUser2 = new Landlord
                {
                    Username = "landlord1",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,

                };
                LandlordUser2.Emails = new List<Email> { email };
                email.owner = LandlordUser2;
                email.ownerUsername = LandlordUser2.Username;

                dbContext.Landlords.Add(LandlordUser2);
                dbContext.SaveChanges();
            }


            var house = dbContext.Houses.FirstOrDefault(h => h.HouseId == "H1");
            if (house == null)
            {
                var house2 = new House
                {
                    HouseId = "H1",
                    
                       governorate = Governorate.Cairo,
                       city = "Cairo",
                       street = "Street 1",

                    LandlordUsername = "landlord1",
                };
                dbContext.Houses.Add(house2);
                dbContext.SaveChanges();
            }
        }
    }

}
