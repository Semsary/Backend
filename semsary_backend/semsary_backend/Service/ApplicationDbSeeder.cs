using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Models;
using System.Net;

namespace semsary_backend.Service
{
    public static class ApplicationDbSeeder
    {
        public static void SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<ApiContext>();

#region Admin
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
#endregion

#region Customer Service
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

                var CustomerServiceUser1 = new CustomerService
                {
                    Username = "CustomerService1",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,

                };
                var CustomerServiceUser2 = new CustomerService
                {
                    Username = "CustomerService2",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,

                };

                var CustomerServiceUser3 = new CustomerService
                {
                    Username = "CustomerService3",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,

                };

                CustomerServiceUser2.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                CustomerServiceUser2.Emails = new List<Email> { email };
                email.owner = CustomerServiceUser2;
                email.ownerUsername = CustomerServiceUser2.Username;

                dbContext.CustomerServices.Add(CustomerServiceUser1);
                dbContext.CustomerServices.Add(CustomerServiceUser2);
                dbContext.CustomerServices.Add(CustomerServiceUser3);

                dbContext.SaveChanges();
            }
#endregion

#region Landlord
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
                var LandlordUser1 = new Landlord
                {
                    Username = "landlord1",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,

                };

                var LandlordUser2 = new Landlord
                {
                    Username = "landlord2",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,

                };

                var LandlordUser3 = new Landlord
                {
                    Username = "landlord3",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,

                };

                LandlordUser2.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                LandlordUser2.Emails = new List<Email> { email };
                email.owner = LandlordUser2;
                email.ownerUsername = LandlordUser2.Username;

                dbContext.Landlords.Add(LandlordUser2);
                dbContext.Landlords.Add(LandlordUser1);
                dbContext.Landlords.Add(LandlordUser3);
                dbContext.SaveChanges();
            }
#endregion

#region Tenant
            string TenantEmail = "Tenant@gmail.com";
            string TenantPassword = "jhg765*&JHy";

            var TenantUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == TenantEmail));

            if (TenantUser == null)
            {
                var passwordHash = PasswordHelper.HashPassword(TenantPassword);

                var email = new Email
                {
                    email = TenantEmail,
                    IsVerified = true
                };
                var Tenant1 = new Tenant
                {
                    Username = "tenant1",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Tenant,

                };

                var Tenant2 = new Tenant
                {
                    Username = "tenant2",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Tenant,

                };

                var Tenant3 = new Tenant
                {
                    Username = "tenant3",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Tenant,

                };


                Tenant2.Emails = new List<Email> { email };
                Tenant2.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                email.owner = Tenant2;
                email.ownerUsername = Tenant2.Username;

                dbContext.Tenant.Add(Tenant2);
                dbContext.Tenant.Add(Tenant1);
                dbContext.Tenant.Add(Tenant3);
                dbContext.SaveChanges();
            }
#endregion

#region House
            var house = dbContext.Houses.FirstOrDefault(h => h.HouseId == "H1");
            if (house == null)
            {
                var house2 = new House
                {
                    HouseId = "H1",
                    LandlordUsername = "landlord1",
                    HouseInspections = new()
                };
                var house3 = new House
                {
                    HouseId = "H2",
                    LandlordUsername = "landlord1",
                    HouseInspections=new()
                };
                dbContext.Houses.Add(house2);
                dbContext.Houses.Add(house3);
                dbContext.SaveChanges();
            }
            #endregion

#region indpiction
            var indpect = new HouseInspection()
            {
                HouseId="H1",
                HouseImages=new(),
                HouseInspectionId="In1",
                inspectionStatus= InspectionStatus.Completed,
                price=0,
                NumberOfBeds=5,
                InspectorId= "CustomerService1",
                InspectionRequestDate=DateTime.UtcNow,
                InspectionDate=DateTime.UtcNow,
                
            };
            dbContext.HouseInspections.Add(indpect);
            dbContext.SaveChanges();
#endregion

#region Rental
            var rental = dbContext.Rentals.FirstOrDefault(r => r.RentalId == 1);
            Rental rental2 = null;
            if (rental == null)
            {
                 rental2 = new Rental
                {
                    RentalId = 1,
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(30),
                    StartArrivalDate = DateTime.UtcNow.AddDays(1),
                    EndArrivalDate = DateTime.UtcNow.AddDays(30),
                    TenantUsername = "tenant1",
                    HouseId = "H1",
                    CreationDate = DateTime.UtcNow,
                    status = RentalStatus.Bending,
                    RentalType = RentalType.ByHouse,
                    RentalUnitIds = new List<string> { "RentalUnit1" }
                };
                dbContext.Rentals.Add(rental2);
                dbContext.SaveChanges();
            }
#endregion

#region RenalUnit
            var rentalUnit = dbContext.RentalUnits.FirstOrDefault(h => h.RentalUnitId == "RentalUnit1");
            if (rentalUnit == null)
            {
                var rentalUnit1 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit1",
                    AdvertisementId = "Adv1",
                    MonthlyCost = 1000,
                    DailyCost = 100,

                };
                var rentalUnit2 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit2",
                    AdvertisementId = "Adv1",
                    MonthlyCost = 1000,
                    DailyCost = 100,
                };
                rentalUnit1.Rentals = new List<Rental> {rental2 };
                dbContext.RentalUnits.Add(rentalUnit1);
                dbContext.RentalUnits.Add(rentalUnit2);
                dbContext.SaveChanges();
            }
#endregion

#region Complaint
        var complaint = dbContext.Complaints.FirstOrDefault(c => c.ComplaintId == 1);
            if (complaint == null)
            {
                var complaint2 = new Complaint
                {
                    ComplaintDetails = "This is a test complaint",
                    SubmittedBy = "tenant1",
                    SubmittingDate = DateTime.UtcNow,
                    status = ComplainStatus.InProgress,
                    RentalId = 1
                };
                dbContext.Complaints.Add(complaint2);
                dbContext.SaveChanges();
            }



            #endregion

        }
    }

}
