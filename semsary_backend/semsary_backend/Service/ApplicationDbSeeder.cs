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
            string CustomerServiceEmail1 = "CustomerService1@gmail.com";
            string CustomerServiceEmail2 = "CustomerService2@gmail.com";
            string CustomerServiceEmail3 = "CustomerService3@gmail.com";
            string CustomerServicePassword = "jhg765*&JHy";

            var CustomerServiceUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == CustomerServiceEmail1));
            if (CustomerServiceUser == null)
            {
                // Hash the password (don't store plain text)
                var passwordHash = PasswordHelper.HashPassword(CustomerServicePassword);

                var email1 = new Email
                {
                    email = CustomerServiceEmail1,
                    IsVerified = true,
                    ownerUsername = "CustomerService1"

                };

                var email2 = new Email
                {
                    email = CustomerServiceEmail2,
                    IsVerified = true,
                    ownerUsername = "CustomerService2"

                };

                var email3 = new Email
                {
                    email = CustomerServiceEmail3,
                    IsVerified = true,
                    ownerUsername = "CustomerService3"


                };


                var CustomerServiceUser1 = new CustomerService
                {
                    Username = "CustomerService1",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,
                    Emails = new List<Email> { email1 },

                };
                var CustomerServiceUser2 = new CustomerService
                {
                    Username = "CustomerService2",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,
                    Emails = new List<Email> { email2 },

                };

                var CustomerServiceUser3 = new CustomerService
                {
                    Username = "CustomerService3",
                    Firstname = "b",
                    Lastname = "b",
                    password = passwordHash,
                    UserType = Enums.UserType.Customerservice,
                    Emails = new List<Email> { email3 },

                };

                CustomerServiceUser2.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                
                dbContext.CustomerServices.Add(CustomerServiceUser1);
                dbContext.CustomerServices.Add(CustomerServiceUser2);
                dbContext.CustomerServices.Add(CustomerServiceUser3);

                dbContext.SaveChanges();
            }
#endregion

#region Landlord
            string LandlordEmail1 = "landlord1@gmail.com";
            string LandlordPassword = "jhg765*&JHy";
            string LandlordEmail2 = "landlord2@gmail.com";
            string LandlordEmail3 = "landlord3@gmail.com";

            var LandlordUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == LandlordEmail1));

            if(LandlordUser == null)
            {
                var passwordHash = PasswordHelper.HashPassword(LandlordPassword);

                var email1 = new Email
                {
                    email = LandlordEmail1,
                    IsVerified = true,
                    ownerUsername= "landlord1",
                };
                var email2 = new Email
                {
                    email = LandlordEmail2,
                    IsVerified = true,
                    ownerUsername = "landlord2"
                };
                var email3 = new Email
                {
                    email = LandlordEmail3,
                    IsVerified = true,
                    ownerUsername = "landlord3"
                };
                var LandlordUser1 = new Landlord
                {
                    Username = "landlord1",
                    Firstname = "mahmoud",
                    Lastname = "ahmed",
                    SocialId = "7687543456543",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email1 },
                    IsVerified = true
                };

                var LandlordUser2 = new Landlord
                {
                    Username = "landlord2",
                    Firstname = "ali",
                    Lastname = "zenhom",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email2 },

                };

                var LandlordUser3 = new Landlord
                {
                    Username = "landlord3",
                    Firstname = "mohamed",
                    Lastname = "gomaa",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email3 },

                };

                LandlordUser1.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                
                dbContext.Landlords.Add(LandlordUser2);
                dbContext.Landlords.Add(LandlordUser1);
                dbContext.Landlords.Add(LandlordUser3);
                dbContext.SaveChanges();
            }
#endregion

#region Tenant
            string TenantEmail1 = "tenant1@gmail.com";
            string TenantEmail2 = "tenant2@gmail.com";
            string TenantEmail3 = "tenant3@gmail.com";
            string TenantPassword = "jhg765*&JHy";

            var TenantUser = dbContext.SermsaryUsers
                .FirstOrDefault(u => u.Emails.Any(m => m.email == TenantEmail1));

            if (TenantUser == null)
            {
                var passwordHash = PasswordHelper.HashPassword(TenantPassword);

                var email1 = new Email
                {
                    email = TenantEmail1,
                    IsVerified = true,
                    ownerUsername = "tenant1"
                };

                var email2 = new Email
                {
                    email = TenantEmail2,
                    IsVerified = true,
                    ownerUsername = "tenant2"
                };
                var email3 = new Email
                {
                    email = TenantEmail3,
                    IsVerified = true,
                    ownerUsername = "tenant3"
                };


                var Tenant1 = new Tenant
                {
                    Username = "tenant1",
                    Firstname = "ebrahim",
                    Lastname = "ahmed",
                    password = passwordHash,
                    UserType = Enums.UserType.Tenant,
                    Emails = new List<Email> { email1 },

                };

                var Tenant2 = new Tenant
                {
                    Username = "tenant2",
                    Firstname = "mohamed",
                    Lastname = "badaqy",
                    password = passwordHash,
                    UserType = Enums.UserType.Tenant,
                    Emails = new List<Email> { email2 },

                };

                var Tenant3 = new Tenant
                {
                    Username = "tenant3",
                    Firstname = "ahmed",
                    Lastname = "gelany",
                    password = passwordHash,
                    UserType = Enums.UserType.Tenant,
                    Emails = new List<Email> { email3 },

                };


                Tenant2.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                
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
                    HouseInspections =new()
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
                HouseInspectionId = "In1",
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price=0,
                NumberOfBeds = 5,
                InspectorId= "CustomerService1",
                InspectionRequestDate=DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow,
                
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
                    status = ComplainStatus.Bending,
                    RentalId = 1
                };
                dbContext.Complaints.Add(complaint2);
                dbContext.SaveChanges();
            }



            #endregion


            #region adv
            var adv = dbContext.Advertisements.FirstOrDefault(c => c.AdvertisementId == "adv1");
            if(adv == null)
            {
                var adv2 = new Advertisement
                {
                    AdvertisementId = "adv1",
                    HouseId = "H1",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house1",
                    houseDescription = "bla bla bla",

                };
                dbContext.Advertisements.Add(adv2);
                dbContext.SaveChanges();
            }

#endregion
        }
    }

}
