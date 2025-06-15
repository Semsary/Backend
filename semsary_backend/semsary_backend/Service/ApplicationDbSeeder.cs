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
            string LandlordEmail4 = "landlord4@gmail.com";
            string LandlordEmail5 = "landlord5@gmail.com";
            string LandlordEmail6 = "landlord6@gmail.com";
            string LandlordEmail7 = "landlord7@gmail.com";
            string LandlordEmail8 = "landlord8@gmail.com";
            string LandlordEmail9 = "landlord9@gmail.com";
            string LandlordEmail10 = "landlord10@gmail.com";
            string LandlordEmail11 = "landlord11@gmail.com";
            string LandlordEmail12 = "landlord12@gmail.com";
            string LandlordEmail13 = "landlord13@gmail.com";
            string LandlordEmail14 = "landlord14@gmail.com";

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
                var email4 = new Email
                {
                    email = LandlordEmail4,
                    IsVerified = true,
                    ownerUsername = "landlord4"
                };
                var email5 = new Email
                {
                    email = LandlordEmail5,
                    IsVerified = true,
                    ownerUsername = "landlord5"
                };
                var email6 = new Email
                {
                    email = LandlordEmail6,
                    IsVerified = true,
                    ownerUsername = "landlord6"
                };
                var email7 = new Email
                {
                    email = LandlordEmail7,
                    IsVerified = true,
                    ownerUsername = "landlord7"
                };
                var email8 = new Email
                {
                    email = LandlordEmail8,
                    IsVerified = true,
                    ownerUsername = "landlord8"
                };
                var email9 = new Email
                {
                    email = LandlordEmail9,
                    IsVerified = true,
                    ownerUsername = "landlord9"
                };
                var email10 = new Email
                {
                    email = LandlordEmail10,
                    IsVerified = true,
                    ownerUsername = "landlord10"
                };
                var email11 = new Email
                {
                    email = LandlordEmail11,
                    IsVerified = true,
                    ownerUsername = "landlord11"
                };
                var email12 = new Email
                {
                    email = LandlordEmail12,
                    IsVerified = true,
                    ownerUsername = "landlord12"
                };
                var email13 = new Email
                {
                    email = LandlordEmail13,
                    IsVerified = true,
                    ownerUsername = "landlord13"
                };
                var email14 = new Email
                {
                    email = LandlordEmail14,
                    IsVerified = true,
                    ownerUsername = "landlord14"
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
                var LandlordUser4 = new Landlord
                {
                    Username = "landlord4",
                    Firstname = "user4",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email4 },
                };

                var LandlordUser5 = new Landlord
                {
                    Username = "landlord5",
                    Firstname = "user5",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email5 },
                };

                var LandlordUser6 = new Landlord
                {
                    Username = "landlord6",
                    Firstname = "user6",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email6 },
                };

                var LandlordUser7 = new Landlord
                {
                    Username = "landlord7",
                    Firstname = "user7",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email7 },
                };

                var LandlordUser8 = new Landlord
                {
                    Username = "landlord8",
                    Firstname = "user8",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email8 },
                };

                var LandlordUser9 = new Landlord
                {
                    Username = "landlord9",
                    Firstname = "user9",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email9 },
                };

                var LandlordUser10 = new Landlord
                {
                    Username = "landlord10",
                    Firstname = "user10",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email10 },
                };

                var LandlordUser11 = new Landlord
                {
                    Username = "landlord11",
                    Firstname = "user11",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email11 },
                };

                var LandlordUser12 = new Landlord
                {
                    Username = "landlord12",
                    Firstname = "user12",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email12 },
                };

                var LandlordUser13 = new Landlord
                {
                    Username = "landlord13",
                    Firstname = "user13",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email13 },
                };

                var LandlordUser14 = new Landlord
                {
                    Username = "landlord14",
                    Firstname = "user14",
                    Lastname = "test",
                    password = passwordHash,
                    UserType = Enums.UserType.landlord,
                    Emails = new List<Email> { email14 },
                };

      
                LandlordUser1.DeviceTokens = new List<string> { "emuyK_OBTO1FbT1qiCuahf:APA91bECm-AVmGgbL75lHgX13u3xtAmx44TTcwdjtTp4WLRvDdJHEMs5NlAO5j8erEdyDVKJg0bCcyKkGcaEzOfkGJhJo7LcZZ326QptG-6THDSw21y37pk" };
                
                dbContext.Landlords.Add(LandlordUser2);
                dbContext.Landlords.Add(LandlordUser1);
                dbContext.Landlords.Add(LandlordUser3);
                dbContext.Landlords.Add(LandlordUser4);
                dbContext.Landlords.Add(LandlordUser5);
                dbContext.Landlords.Add(LandlordUser6);
                dbContext.Landlords.Add(LandlordUser7);
                dbContext.Landlords.Add(LandlordUser8);
                dbContext.Landlords.Add(LandlordUser9);
                dbContext.Landlords.Add(LandlordUser10);
                dbContext.Landlords.Add(LandlordUser11);
                dbContext.Landlords.Add(LandlordUser12);
                dbContext.Landlords.Add(LandlordUser13);
                dbContext.Landlords.Add(LandlordUser14);
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
                    governorate = Governorate.Cairo,
                    HouseInspections = new()
                };
                var house3 = new House
                {
                    HouseId = "H2",
                    LandlordUsername = "landlord1",
                    governorate = Governorate.Giza,
                    HouseInspections =new()
                };
                var house4 = new House
                {
                    HouseId = "H3",
                    LandlordUsername = "landlord2",
                    governorate = Governorate.Dakahlia,
                    HouseInspections = new()
                };
                var house5 = new House
                {
                    HouseId = "H4",
                    LandlordUsername = "landlord3",
                    governorate = Governorate.Damietta,
                    HouseInspections = new()
                };

                var house6 = new House
                {
                    HouseId = "H5",
                    LandlordUsername = "landlord4",
                    governorate = Governorate.Alexandria,
                    HouseInspections = new()
                };

                var house7 = new House
                {
                    HouseId = "H6",
                    LandlordUsername = "landlord5",
                    governorate = Governorate.Giza,
                    HouseInspections = new()
                };

                var house8 = new House
                {
                    HouseId = "H7",
                    LandlordUsername = "landlord6",
                    governorate = Governorate.BeniSuef,
                    HouseInspections = new()
                };

                var house9 = new House
                {
                    HouseId = "H8",
                    governorate =Governorate.Qalyubia,
                    LandlordUsername = "landlord7",
                    HouseInspections = new()
                };

                var house10 = new House
                {
                    HouseId = "H9",
                    governorate = Governorate.Minya,
                    LandlordUsername = "landlord8",
                    HouseInspections = new()
                };

                var house11 = new House
                {
                    HouseId = "H10",
                    governorate = Governorate.Giza,
                    LandlordUsername = "landlord9",
                    HouseInspections = new()
                };

                var house12 = new House
                {
                    HouseId = "H11",
                    governorate = Governorate.Luxor,
                    LandlordUsername = "landlord10",
                    HouseInspections = new()
                };

                var house13 = new House
                {
                    HouseId = "H12",
                    governorate = Governorate.SouthSinai,
                    LandlordUsername = "landlord11",
                    HouseInspections = new()
                };

                var house14 = new House
                {
                    HouseId = "H13",
                    governorate = Governorate.Aswan,
                    LandlordUsername = "landlord12",
                    HouseInspections = new()
                };

                var house15 = new House
                {
                    HouseId = "H14",
                    governorate = Governorate.Cairo,
                    LandlordUsername = "landlord13",
                    HouseInspections = new()
                };

                var house16 = new House
                {
                    HouseId = "H15",
                    governorate = Governorate.NewValley,
                    LandlordUsername = "landlord14",
                    HouseInspections = new()
                };

                dbContext.Houses.Add(house2);
                dbContext.Houses.Add(house3);
                dbContext.Houses.Add(house4);
                dbContext.Houses.Add(house5);
                dbContext.Houses.Add(house6);
                dbContext.Houses.Add(house7);
                dbContext.Houses.Add(house8);
                dbContext.Houses.Add(house9);
                dbContext.Houses.Add(house10);
                dbContext.Houses.Add(house11);
                dbContext.Houses.Add(house12);
                dbContext.Houses.Add(house13);
                dbContext.Houses.Add(house14);
                dbContext.Houses.Add(house15);
                dbContext.Houses.Add(house16);
                dbContext.SaveChanges();
            }
            #endregion

#region indpiction
            var indpect1 = new HouseInspection()
            {
                HouseId="H1",
                HouseImages=new(),
                HouseInspectionId = "In1",
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 1000,
                NumberOfBeds = 5,
                NumberOfPathRooms = 2,
                NumberOfBedRooms =6,
                NumberOfChairs = 4,
                NumberOfTables = 2,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 2,
                FloorNumber = 3,
                HouseFeature = HouseFeature.HaveElevator | HouseFeature.Havekitchen | HouseFeature.HaveTV | HouseFeature.HaveWiFi,
                InspectorId = "CustomerService1",
                InspectionRequestDate=DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow,
            };
            var inspect2 = new HouseInspection()
            {
                HouseId = "H2",
                HouseImages = new(),
                HouseInspectionId = "In2",
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 8000,
                NumberOfBeds = 4,
                NumberOfPathRooms = 1,
                NumberOfBedRooms = 2,
                NumberOfChairs = 9,
                NumberOfTables = 2,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 2,
                FloorNumber = 3,
                HouseFeature = HouseFeature.HaveHeater | HouseFeature.Havekitchen | HouseFeature.HaveSalon | HouseFeature.HaveWiFi,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow,
            };
            var inspect3 = new HouseInspection()
            {
                HouseId = "H3",
                HouseImages = new(),
                HouseInspectionId = "In3",
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 5000,
                NumberOfBeds = 3,
                NumberOfPathRooms = 1,
                NumberOfBedRooms = 3,
                NumberOfChairs = 5,
                NumberOfTables = 1,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 1,
                FloorNumber = 2,
                HouseFeature = HouseFeature.HaveTV | HouseFeature.HaveWiFi,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow,
            };

            var inspect4 = new HouseInspection()
            {
                HouseId = "H4",
                HouseImages = new(),
                HouseInspectionId = "In4",
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 10000,
                NumberOfBeds = 2,
                NumberOfPathRooms = 1,
                NumberOfBedRooms = 2,
                NumberOfChairs = 4,
                NumberOfTables = 1,
                NumberOfBalacons = 0,
                NumberOfAirConditionnar = 1,
                FloorNumber = 1,
                HouseFeature = HouseFeature.Havekitchen | HouseFeature.HaveWiFi | HouseFeature.HaveNearGym | HouseFeature.HaveNearPlayGround ,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow,
            };
            var inspect5 = new HouseInspection() {
                HouseId = "H5",
                HouseInspectionId = "In5",
                HouseImages = new(),
                longitude = "31.2345",
                latitude = "30.1234", 
                inspectionStatus = InspectionStatus.Aproved,
                price = 7000,
                NumberOfBeds = 2,
                NumberOfPathRooms = 1, 
                NumberOfBedRooms = 2,
                NumberOfChairs = 4,
                NumberOfTables = 1, 
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 1, 
                FloorNumber = 1, 
                HouseFeature = HouseFeature.HaveWiFi | HouseFeature.HaveTV | HouseFeature.HaveNearGym | HouseFeature.HaveNearUniversity,
                InspectorId = "CustomerService1", 
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow
            };
            var inspect6 = new HouseInspection()
            {
                HouseId = "H6",
                HouseInspectionId = "In6",
                HouseImages = new(),
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 5000,
                NumberOfBeds = 3,
                NumberOfPathRooms = 1,
                NumberOfBedRooms = 2,
                NumberOfChairs = 4,
                NumberOfTables = 1,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 3,
                FloorNumber = 1,
                HouseFeature = HouseFeature.HaveWiFi | HouseFeature.HaveTV | HouseFeature.HaveNearGym | HouseFeature.HaveNearUniversity,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow
            };
            var inspect7 = new HouseInspection()
            {
                HouseId = "H7",
                HouseInspectionId = "In7",
                HouseImages = new(),
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 5000,
                NumberOfBeds = 3,
                NumberOfPathRooms = 1,
                NumberOfBedRooms = 2,
                NumberOfChairs = 4,
                NumberOfTables = 1,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 3,
                FloorNumber = 1,
                HouseFeature = HouseFeature.HaveWiFi | HouseFeature.HaveTV | HouseFeature.HaveNearGym | HouseFeature.HaveNearUniversity,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow
            };
            var inspect8 = new HouseInspection()
            {
                HouseId = "H8",
                HouseInspectionId = "In8",
                HouseImages = new(),
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 5000,
                NumberOfBeds = 3,
                NumberOfPathRooms = 1,
                NumberOfBedRooms = 2,
                NumberOfChairs = 4,
                NumberOfTables = 1,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 3,
                FloorNumber = 1,
                HouseFeature = HouseFeature.HaveWiFi | HouseFeature.HaveTV | HouseFeature.HaveNearGym | HouseFeature.HaveNearUniversity,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow
            };
            var inspect9 = new HouseInspection()
            {
                HouseId = "H9",
                HouseInspectionId = "In9",
                HouseImages = new(),
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 7000,
                NumberOfBeds = 3,
                NumberOfPathRooms = 2,
                NumberOfBedRooms = 2,
                NumberOfChairs = 9,
                NumberOfTables = 2,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 3,
                FloorNumber = 1,
                HouseFeature = HouseFeature.Havekitchen | HouseFeature.HaveTV | HouseFeature.HaveNearGym | HouseFeature.HaveNearUniversity,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow
            };
            var inspect10 = new HouseInspection()
            {
                HouseId = "H10",
                HouseInspectionId = "In10",
                HouseImages = new(),
                longitude = "31.2345",
                latitude = "30.1234",
                inspectionStatus = InspectionStatus.Aproved,
                price = 7000,
                NumberOfBeds = 3,
                NumberOfPathRooms = 2,
                NumberOfBedRooms = 2,
                NumberOfChairs = 9,
                NumberOfTables = 2,
                NumberOfBalacons = 1,
                NumberOfAirConditionnar = 3,
                FloorNumber = 1,
                HouseFeature = HouseFeature.Havekitchen | HouseFeature.HaveTV | HouseFeature.HaveNearGym | HouseFeature.HaveNearUniversity,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
                InspectionDate = DateTime.UtcNow
            };
            var inspect11 = new HouseInspection()
            {
                HouseId = "H11",
                HouseInspectionId = "In11",
                inspectionStatus = InspectionStatus.InProgress,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
            };
            var inspect12 = new HouseInspection()
            {
                HouseId = "H12",
                HouseInspectionId = "In12",
                inspectionStatus = InspectionStatus.InProgress,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
            };
            var inspect13 = new HouseInspection()
            {
                HouseId = "H13",
                HouseInspectionId = "In13",
                inspectionStatus = InspectionStatus.InProgress,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
            };
            var inspect14 = new HouseInspection()
            {
                HouseId = "H14",
                HouseInspectionId = "In14",
                inspectionStatus = InspectionStatus.InProgress,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
            };
            var inspect15 = new HouseInspection()
            {
                HouseId = "H15",
                HouseInspectionId = "In15",
                inspectionStatus = InspectionStatus.InProgress,
                InspectorId = "CustomerService1",
                InspectionRequestDate = DateTime.UtcNow,
            };
            dbContext.HouseInspections.Add(indpect1);
            dbContext.HouseInspections.Add(inspect2);
            dbContext.HouseInspections.Add(inspect3);
            dbContext.HouseInspections.Add(inspect4);
            dbContext.HouseInspections.Add(inspect5);
            dbContext.HouseInspections.Add(inspect6);
            dbContext.HouseInspections.Add(inspect7);
            dbContext.HouseInspections.Add(inspect8);
            dbContext.HouseInspections.Add(inspect9);
            dbContext.HouseInspections.Add(inspect10);
            dbContext.HouseInspections.Add(inspect11);
            dbContext.HouseInspections.Add(inspect12);
            dbContext.HouseInspections.Add(inspect13);
            dbContext.HouseInspections.Add(inspect14);
            dbContext.HouseInspections.Add(inspect15);
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
                    MonthlyCost = 5000,
                    DailyCost = 100,

                };
                var rentalUnit2 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit2",
                    AdvertisementId = "Adv2",
                    MonthlyCost = 7000,
                    DailyCost = 300,
                };
                var rentalUnit3 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit3",
                    AdvertisementId = "Adv3",
                    MonthlyCost = 6500,
                    DailyCost = 250,
                };

                var rentalUnit4 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit4",
                    AdvertisementId = "Adv4",
                    MonthlyCost = 4000,
                    DailyCost = 150,
                };

                var rentalUnit5 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit5",
                    AdvertisementId = "Adv5",
                    MonthlyCost = 8000,
                    DailyCost = 350,
                };

                var rentalUnit6 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit6",
                    AdvertisementId = "Adv6",
                    MonthlyCost = 7200,
                    DailyCost = 300,
                };

                var rentalUnit7 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit7",
                    AdvertisementId = "Adv7",
                    MonthlyCost = 5000,
                    DailyCost = 180,
                };

                var rentalUnit8 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit8",
                    AdvertisementId = "Adv8",
                    MonthlyCost = 6000,
                    DailyCost = 200,
                };

                var rentalUnit9 = new RentalUnit
                {
                    RentalUnitId = "RentalUnit9",
                    AdvertisementId = "Adv9",
                    MonthlyCost = 5800,
                    DailyCost = 220,
                };

                rentalUnit1.Rentals = new List<Rental> {rental2 };
                dbContext.RentalUnits.Add(rentalUnit1);
                dbContext.RentalUnits.Add(rentalUnit2);
                dbContext.RentalUnits.Add(rentalUnit3);
                dbContext.RentalUnits.Add(rentalUnit4);
                dbContext.RentalUnits.Add(rentalUnit5);
                dbContext.RentalUnits.Add(rentalUnit6);
                dbContext.RentalUnits.Add(rentalUnit7);
                dbContext.RentalUnits.Add(rentalUnit8);
                dbContext.RentalUnits.Add(rentalUnit9);
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
                    houseDescription = "greate view",

                };
                var adv3 = new Advertisement
                {
                    AdvertisementId = "adv2",
                    HouseId = "H2",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house2",
                    houseDescription = "quiet neighborhood",
                };
                var adv4 = new Advertisement
                {
                    AdvertisementId = "adv3",
                    HouseId = "H3",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house3",
                    houseDescription = "spacious and sunny",
                };

                var adv5 = new Advertisement
                {
                    AdvertisementId = "adv4",
                    HouseId = "H4",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house4",
                    houseDescription = "near public transport",
                };

                var adv6 = new Advertisement
                {
                    AdvertisementId = "adv5",
                    HouseId = "H5",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house5",
                    houseDescription = "quiet neighborhood",
                };

                var adv7 = new Advertisement
                {
                    AdvertisementId = "adv6",
                    HouseId = "H6",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house6",
                    houseDescription = "recently renovated",
                };

                var adv8 = new Advertisement
                {
                    AdvertisementId = "adv7",
                    HouseId = "H7",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house7",
                    houseDescription = "close to school",
                };

                var adv9 = new Advertisement
                {
                    AdvertisementId = "adv8",
                    HouseId = "H8",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house8",
                    houseDescription = "great view and balcony",
                };
                var adv10 = new Advertisement
                {
                    AdvertisementId = "adv9",
                    HouseId = "H9",
                    PublishDate = DateTime.UtcNow,
                    HouseName = "house9",
                    houseDescription = "spacious and sunny",
                };
                dbContext.Advertisements.Add(adv2);
                dbContext.Advertisements.Add(adv3);
                dbContext.Advertisements.Add(adv4);
                dbContext.Advertisements.Add(adv5);
                dbContext.Advertisements.Add(adv6);
                dbContext.Advertisements.Add(adv7);
                dbContext.Advertisements.Add(adv8);
                dbContext.Advertisements.Add(adv9);
                dbContext.Advertisements.Add(adv10);
                dbContext.SaveChanges();
            }

#endregion
        }
    }

}
