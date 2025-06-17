using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Esf;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Models;
using semsary_backend.Service;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantController(TokenService tokenGenertor, ApiContext apiContext, NotificationService notificationService, RecommendationSystem recommendation) : ControllerBase
    {
        [HttpPost("Make/Complaint/{rentalId}")]
        public async Task<IActionResult> MakeComplaint(string rentalId, [FromBody] ComplaintRequestForTentatDTO complaintRequestDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return Unauthorized();

            if (!user.IsVerified)
                return Forbid();

            if (complaintRequestDTO == null)
                return BadRequest(new { message = "Invalid data." });

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            int id;
            if (!int.TryParse(rentalId, out id))
                return BadRequest(new { message = "Invalid data format." });

            var rental = await apiContext.Rentals.FindAsync(id);
            if (rental == null)
                return NotFound(new { message = "There is no rental found with this id" });

            if (rental.TenantUsername != user.Username)
                return Forbid();

            var complaint = new Complaint
            {
                SubmittedBy = user.Username,
                SubmittingDate = DateTime.UtcNow,
                RentalId = id,
                ComplaintDetails = complaintRequestDTO.ComplaintDetails,
            };

            await apiContext.Complaints.AddAsync(complaint);
            await apiContext.SaveChangesAsync();

            return Ok(new { message = "Complaint submitted successfully" });
        }

        [HttpPost("Set/Rate/{houseId}")]
        public async Task<IActionResult> SetRate(string houseId, [FromBody] RateDTO rateDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return Unauthorized();

            if (!user.IsVerified)
                return Forbid();

            if (rateDTO == null)
                return BadRequest(new { message = "Invalid data." });

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var house = await apiContext.Houses
                .Include(r => r.Rentals)
                .FirstOrDefaultAsync(h => h.HouseId == houseId);
            if (house == null)
                return NotFound(new { message = "There is no house found with this id" });

            var rental = house.Rentals
                .Where(r => r.status == Enums.RentalStatus.ArrivalAccept)
                .FirstOrDefault(r => r.TenantUsername == user.Username);

            if (rental == null)
                return Forbid();

            if (rateDTO.StarsNumber > 5 || rateDTO.StarsNumber < 0)
                return BadRequest(new { message = "Number of stars." });


            var pastRate = await apiContext.Rates
                .Where(r => r.HouseId == houseId && r.TenantUsername == user.Username)
                .FirstOrDefaultAsync();

            if (pastRate == null)
            {
                var rate = new Rate
                {
                    HouseId = houseId,
                    TenantUsername = user.Username,
                    RateDate = DateTime.UtcNow,
                    StarsNumber = rateDTO.StarsNumber,
                };

                var rates = apiContext.Rates
                    .Where(h => h.HouseId == houseId)
                    .ToList();

                house.AvrageRate = rates.Any() ? rates.Average(rate => rate.StarsNumber) : rateDTO.StarsNumber;
                house.NumOfRaters++;

                await apiContext.Rates.AddAsync(rate);
                await apiContext.SaveChangesAsync();
            }
            else
            {
                pastRate.StarsNumber = rateDTO.StarsNumber;
                pastRate.RateDate = DateTime.UtcNow;

                var rates = apiContext.Rates
                    .Where(h => h.HouseId == houseId)
                    .ToList();

                house.AvrageRate = rates.Any() ? rates.Average(rate => rate.StarsNumber) : rateDTO.StarsNumber;
                await apiContext.SaveChangesAsync();
            }
            return Ok(new { message = "Rate submitted successfully" });
        }

        [HttpPost("Add/Comment/{houseId}")]
        public async Task<IActionResult> AddComment(string houseId, [FromBody] CommentDTO rateDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return Unauthorized();

            if (!user.IsVerified)
                return Forbid();

            if (rateDTO == null)
                return BadRequest(new { message = "Invalid data." });

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var house = await apiContext.Houses
                .Include(r => r.Rentals)
                .FirstOrDefaultAsync(h => h.HouseId == houseId);

            if (house == null)
                return NotFound(new { message = "There is no house found with this id" });

            var rental = house.Rentals
                .Where(r => r.status == Enums.RentalStatus.ArrivalAccept)
                .FirstOrDefault(r => r.TenantUsername == user.Username);

            if (rental == null)
                return Forbid();

            if (rental.NumOfComments > 10)
                return BadRequest(new { message = "You cannot add more than 10 comments for the same rental." });

            var Comment = new Comment
            {
                HouseId = houseId,
                TenantUsername = user.Username,
                CommentDate = DateTime.UtcNow,
                CommentDetails = rateDTO.CommentDetails,
            };
            await apiContext.AddAsync(Comment);
            await apiContext.SaveChangesAsync();
            rental.NumOfComments++;
            return Ok(new { message = "Comment added successfully" });
        }

        [HttpPost("show/available/rentalUnits")]
        public async Task<IActionResult> ShowAvailableRentalUnits([FromBody]RentalPeriodDTO rentalDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            if (rentalDTO == null)
                return BadRequest(new { message = "Invalid data." });

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var adv = await apiContext.Advertisements
            .Include(a => a.RentalUnits)
                .ThenInclude(u => u.Rentals)
            .FirstOrDefaultAsync(a => a.AdvertisementId == rentalDTO.AdvId);

            if (adv == null)
                return BadRequest("There is no advertisement with this id");

            DateTime currentDate = DateTime.UtcNow;
            if (rentalDTO.StartDate < currentDate || rentalDTO.EndDate < currentDate)
                return BadRequest(new { message = "The rental period must be in the future." });

            if (rentalDTO.StartDate > rentalDTO.EndDate)
                return BadRequest(new { message = "The beginning of rental period cannot be after its end." });

            if (rentalDTO.StartArrivalDate > rentalDTO.EndDate || rentalDTO.EndArrivalDate > rentalDTO.EndDate)
                return BadRequest(new { message = "Invalid arrival period, you cannot arrive after your rental period ends." });

            if (rentalDTO.StartArrivalDate < currentDate || rentalDTO.EndArrivalDate < currentDate)
                return BadRequest(new { message = "The arrival period must be in the future." });

            if (rentalDTO.StartArrivalDate > rentalDTO.EndArrivalDate)
                return BadRequest(new { message = "The beginning of arrival period cannot be after its end." });

            var conflictsPerUnit = adv.RentalUnits
                .Select(unit => new
                {
                    RentalUnitId = unit.RentalUnitId,
                    HasConflict = unit.Rentals.Any(r =>
                        rentalDTO.StartDate < r.EndDate && rentalDTO.EndDate > r.StartDate && !(r.status == RentalStatus.Bending || r.status == RentalStatus.Rejected || r.status==RentalStatus.ArrivalReject))
                })
                .ToList();
            return Ok(conflictsPerUnit);
        }

        [HttpPost("Make/Rental/Request/")]
        public async Task<IActionResult> MakeRentalRequest(RentalDTO rentalDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            if (!user.IsVerified)
                return Forbid();

            if (rentalDTO == null)
                return BadRequest(new { message = "Invalid data." });

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var house = apiContext.Houses.Include(h => h.owner).Where(h => h.HouseId == rentalDTO.HouseId).FirstOrDefault();
            if (house == null)
                return NotFound();

            var rentalUnits = await apiContext.RentalUnits
                .Where(u => rentalDTO.RentalUnitIds.Contains(u.RentalUnitId))
                .Include(u => u.Rentals)
                .ToListAsync();

            TimeSpan difference = rentalDTO.EndDate - rentalDTO.StartDate;
            int WarrantyCost = 0;
            if (difference.Days >= 30)
            {
                foreach (var rent in rentalUnits)
                    WarrantyCost += rent.MonthlyCost / 2;
            }
            else
            {
                int numOfWarrantyDays = 0;

                if (difference.Days <= 10)
                    numOfWarrantyDays = 1;
                else if (difference.Days <= 20)
                    numOfWarrantyDays = 2;
                else
                    numOfWarrantyDays = 3;

                foreach (var rent in rentalUnits)
                    WarrantyCost += rent.DailyCost * numOfWarrantyDays;
            }
            var rental = new Rental
            {
                StartDate = rentalDTO.StartDate,
                EndDate = rentalDTO.EndDate,
                StartArrivalDate = rentalDTO.StartArrivalDate,
                EndArrivalDate = rentalDTO.EndArrivalDate,
                HouseId = rentalDTO.HouseId,
                RentalType = rentalDTO.RentalType,
                RentalUnitIds = rentalDTO.RentalUnitIds,
                WarrantyMoney = WarrantyCost,
                TenantUsername = user.Username,
                CreationDate = DateTime.UtcNow,
                status = Enums.RentalStatus.Bending
            };

            await apiContext.Rentals.AddAsync(rental);
            Landlord lanlord = house.owner;

            string title = "طلب تأجير جديد";
            string message = $"لقد حصلت علي طلب تأجير جديد من {user.Firstname} {user.Lastname}\nللمزيد من التفاصيل قم بزيارة الموقع.";

            var notification = new Models.Notification
            {
                Title = title,
                Message = message,
                SentTo = lanlord.Username,
                CreatedAt = DateTime.UtcNow,
            };
            apiContext.Notifications.Add(notification);
            await apiContext.SaveChangesAsync();

            await notificationService.SendNotificationAsync(title, message, lanlord);

            return Ok(new
            {
                warrantyMoney = rental.WarrantyMoney,
                rentalId = rental.RentalId
            });
        }

        [HttpPost("Cancel/rental/Request/{rentId}")]
        public async Task<IActionResult> CancelRentalRequest(string rentId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            int rentalId;
            if (!int.TryParse(rentId, out rentalId))
                return BadRequest(new { message = "Invalid rental ID format." });

            var rental = await apiContext.Rentals.FindAsync(rentalId);
            if (rental == null)
                return NotFound(new { message = "There is no rental found with this id" });

            if (rental.TenantUsername != user.Username)
                return Forbid();

            var house = await apiContext.Houses.FindAsync(rental.HouseId);
            if (house == null)
                return NotFound(new { message = "There is no house found with this id" });
            var lanlord = house.owner;

            if (rental.status == Enums.RentalStatus.Bending)
            {
                apiContext.Rentals.Remove(rental);
                await apiContext.SaveChangesAsync();
                return Ok(new { message = "Rental request cancelled successfully" });
            }
            if (rental.status == Enums.RentalStatus.Accepted && rental.ResponseDate.AddDays(2) < DateTime.UtcNow)
            {
                lanlord.Balance += rental.WarrantyMoney;
                apiContext.Remove(rental);

                string title = "تم الغاء طلب حجز";
                string message = $"قام {user.Firstname} {user.Lastname}\nبإلغاء الحجز بعد الفترة المسموحة\n و قد تم تحويل فلوس الضمان لحسابك.";

                var notification = new Models.Notification
                {
                    Title = title,
                    Message = message,
                    SentTo = lanlord.Username,
                    CreatedAt = DateTime.UtcNow,
                };
                apiContext.Notifications.Add(notification);
                await apiContext.SaveChangesAsync();
                await notificationService.SendNotificationAsync(title,message, lanlord);
                return Ok(new { message = "Rental request cancelled successfully" });
            }
            if (rental.status == Enums.RentalStatus.Accepted && rental.ResponseDate.AddDays(2) >= DateTime.UtcNow)
            {
                user.Balance += (int)(rental.WarrantyMoney - rental.WarrantyMoney * Rental.OurPercentage);
                apiContext.Remove(rental);

                string title = "تم الغاء طلب حجز";
                string message = $"قام {user.Firstname} {user.Lastname}\n بإلغاء الحجز خلال الفترة المسموحة\nتستطيع الآن القيام بعملية التأجير للآخرين في هذه الفترة";

                var notification = new Models.Notification
                {
                    Title = title,
                    Message = message,
                    SentTo = lanlord.Username,
                    CreatedAt = DateTime.UtcNow,
                };
                apiContext.Notifications.Add(notification);

                await apiContext.SaveChangesAsync();
                await notificationService.SendNotificationAsync(title, message, lanlord);
                return Ok(new { message = "Rental request cancelled successfully" });
            }
            return BadRequest();
        }
        [HttpGet("Get/All/Rental/Requests")]
        public async Task<IActionResult> GetAllRentalRequests()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            var rentals = await apiContext.Rentals
                .Where(r => r.TenantUsername == user.Username)
                .OrderByDescending(r => r.CreationDate)
                .Include(r => r.House)
                .Select( r => new
                    { 
                        r.RentalId,
                        HouseName = r.House.Advertisements.Select(a => a.HouseName),
                        r.status,
                        r.StartDate,
                        r.EndDate,
                        r.StartArrivalDate,
                        r.EndArrivalDate,
                        r.WarrantyMoney,
                        RentalUnits = r.House.Advertisements.Select(a => a.RentalUnits
                                .Select(unit => new
                                {
                                    unit.RentalUnitId,
                                    unit.DailyCost,
                                    unit.MonthlyCost,                                 
                                }))
                    }
                ).ToListAsync();

            if (!rentals.Any())
                return NoContent();


            return Ok(rentals);
        }

        [HttpPost("Confirm/Arrival/Request/{rentId}")]
        public async Task<IActionResult> ConfirmArrival(string rentId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            int rentalId;
            if (!int.TryParse(rentId, out rentalId))
                return BadRequest(new { message = "Invalid rental ID format." });

            var rental = await apiContext.Rentals.FindAsync(rentalId);
            if (rental == null)
                return NotFound(new { message = "There is no rental found with this id" });

            if (rental.TenantUsername != user.Username)
                return Forbid();

            if (rental.status == Enums.RentalStatus.Accepted)
            {
                rental.status = Enums.RentalStatus.ArrivalRequest;
                var house = await apiContext.Houses.FindAsync(rental.HouseId);
                if (house == null)
                    return NotFound(new { message = "There is no house found with this id" });
                var lanlord = house.owner;

                string title = "طلب تأكيد الوصول";
                string message = $"قام {user.Firstname} {user.Lastname}\nبطلب تأكيد الوصول قم لتأكيد أو رفض العملية فم بزيارة الموقع";

                var notification = new Models.Notification
                {
                    Title = title,
                    Message = message,
                    SentTo = lanlord.Username,
                    CreatedAt = DateTime.UtcNow,
                };
                apiContext.Notifications.Add(notification);
                await apiContext.SaveChangesAsync();
                await notificationService.SendNotificationAsync(title, message, lanlord);
                return Ok(new { message = "Arrival request sent successfully" });
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("Search")]
        public async Task<IActionResult> SearchHouses([FromQuery] FilterDTO filterDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.FirstOrDefaultAsync(r => r.Username == username);

            var houseQuery = apiContext.Houses
                .Include(h => h.HouseInspections)
                .Include(h => h.Advertisements)
                    .ThenInclude(a => a.RentalUnits)
                .AsQueryable();
            
                if (filterDTO.governorate.HasValue)
                    houseQuery = houseQuery.Where(h => h.governorate == filterDTO.governorate);

                if (filterDTO.MinMonthlyCost.HasValue || filterDTO.MaxMonthlyCost.HasValue)
                {
                    houseQuery = houseQuery.Where(h =>
                        h.Advertisements.Any(ad =>
                            ad.RentalUnits.Any(unit =>
                                (!filterDTO.MinMonthlyCost.HasValue || unit.MonthlyCost >= filterDTO.MinMonthlyCost.Value) &&
                                (!filterDTO.MaxMonthlyCost.HasValue || unit.MonthlyCost <= filterDTO.MaxMonthlyCost.Value)
                            )
                        )
                    );
                }

                if (filterDTO.MinDailyCost.HasValue || filterDTO.MaxDailyCost.HasValue)
                {
                    houseQuery = houseQuery.Where(h =>
                        h.Advertisements.Any(ad =>
                            ad.RentalUnits.Any(unit =>
                                (!filterDTO.MinDailyCost.HasValue || unit.DailyCost >= filterDTO.MinDailyCost.Value) &&
                                (!filterDTO.MaxDailyCost.HasValue || unit.DailyCost <= filterDTO.MaxDailyCost.Value)
                            )
                        )
                    );
                }

                if (filterDTO.FloorNumber.HasValue)
                    houseQuery = houseQuery.Where(h => h.HouseInspections.Any(l => l.FloorNumber == filterDTO.FloorNumber.Value));

                if (filterDTO.NumOfBedrooms.HasValue)
                    houseQuery = houseQuery.Where(h => h.HouseInspections.Any(l => l.NumberOfBedRooms == filterDTO.NumOfBedrooms.Value));

                if (filterDTO.NumOfBathrooms.HasValue)
                    houseQuery = houseQuery.Where(h => h.HouseInspections.Any(l => l.NumberOfPathRooms == filterDTO.NumOfBathrooms.Value));

                if (filterDTO.rentalType.HasValue)
                    houseQuery = houseQuery.Where(h => h.Rentals.Any(r => r.RentalType == filterDTO.rentalType.Value));
            
            if (user != null && user.UserType == UserType.Tenant && user.CompletedProfile)
            {
                var houses = await houseQuery
                    .Select(h => new
                    {
                        Advertisements = h.Advertisements.Select(ad => new
                        {
                            ad.AdvertisementId,
                            ad.HouseName,
                            ad.houseDescription,
                            ad.PublishDate,
                            ad.rentalType,
                            RentalUnits = ad.RentalUnits.ToList()
                        }).ToList(),
                        houseRate = h.AvrageRate,
                        NumOfRaters = h.NumOfRaters,
                        LatestInspection = h.HouseInspections
                            .OrderByDescending(i => i.InspectionDate)
                            .FirstOrDefault(),
                        gover = h.governorate,
                        city = h.city,
                        street = h.street
                    })
                    .ToListAsync();

                var sortedAds = houses
                    .OrderByDescending(h => recommendation.recommend(user, h.LatestInspection, h.gover))
                    .SelectMany(h => h.Advertisements, (h, ad) => new
                    {
                        AdvertisementId = ad.AdvertisementId,
                        HouseRate = h.houseRate,
                        NumOfRaters = h.NumOfRaters,
                        HouseName = ad.HouseName,
                        HouseDescription = ad.houseDescription,
                        PublishDate = ad.PublishDate,
                        Governorate = h.gover,
                        City = h.city,
                        Street = h.street,
                        RentalType = ad.rentalType,
                        Images = h.LatestInspection?.HouseImages,
                        DailyCost = ad.RentalUnits.Any() ? ad.RentalUnits.Min(u => u.DailyCost) : 0,
                        MonthlyCost = ad.RentalUnits.Any() ? ad.RentalUnits.Min(u => u.MonthlyCost) : 0,
                    })
                    .ToList();

                return Ok(sortedAds);
            }
            else
            {
                var houses = await houseQuery
                    .Select(h => new
                    {
                        Advertisements = h.Advertisements.Select(ad => new
                        {
                            ad.AdvertisementId,
                            ad.HouseName,
                            ad.houseDescription,
                            ad.PublishDate,
                            ad.rentalType,
                            RentalUnits = ad.RentalUnits.ToList()
                        }).ToList(),
                        houseRate = h.AvrageRate,
                        NumOfRaters = h.NumOfRaters,
                        LatestInspection = h.HouseInspections
                            .OrderByDescending(i => i.InspectionDate)
                            .FirstOrDefault(),
                        gover = h.governorate,
                        city = h.city,
                        street = h.street
                    })
                    .ToListAsync();

                var advs = houses
                    .SelectMany(h => h.Advertisements, (h, ad) => new
                    {
                        AdvertisementId = ad.AdvertisementId,
                        HouseRate = h.houseRate,
                        NumOfRaters = h.NumOfRaters,
                        HouseName = ad.HouseName,
                        HouseDescription = ad.houseDescription,
                        PublishDate = ad.PublishDate,
                        Governorate = h.gover,
                        City = h.city,
                        Street = h.street,
                        RentalType = ad.rentalType,
                        Images = h.LatestInspection?.HouseImages,
                        DailyCost = ad.RentalUnits.Any() ? ad.RentalUnits.Min(u => u.DailyCost) : 0,
                        MonthlyCost = ad.RentalUnits.Any() ? ad.RentalUnits.Min(u => u.MonthlyCost) : 0,
                    })
                    .ToList();

                return Ok(advs);
            }
        }
        [AllowAnonymous]
        [HttpGet("Advertisment/Detials/{advId}")]
        public async Task<IActionResult> AdvDetails(string advId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.FirstOrDefaultAsync(r => r.Username == username);

            var adv = await apiContext.Advertisements
                .Include(a => a.RentalUnits)
                .Include(a => a.House)
                    .ThenInclude(h => h.Comments)
                .Include(a => a.House)
                    .ThenInclude(h => h.HouseInspections)
                .FirstOrDefaultAsync(a => a.AdvertisementId == advId);

            if (adv == null)
                return NotFound(new { message = "Advertisement not found." });


            var HouseInspectionInfo = adv.House.HouseInspections
                .OrderByDescending(i => i.InspectionDate)
                .Select(i => new
                {
                    i.HouseId,
                    i.longitude,
                    i.latitude,
                    i.FloorNumber,
                    i.NumberOfAirConditionnar,
                    i.NumberOfPathRooms,
                    i.NumberOfBedRooms,
                    i.NumberOfBeds,
                    i.NumberOfBalacons,
                    i.NumberOfTables,
                    i.NumberOfChairs,
                    i.InspectionDate,
                    i.HouseFeature,
                    i.HouseImages,
                    estimatedPrice = i.price,
                })
                .FirstOrDefault();

            var RentalUnitInfo = adv.RentalUnits
                .Select(u => new
                {
                    u.RentalUnitId,
                    u.MonthlyCost,
                    u.DailyCost,
                })
                .ToList();

            object Comments = null;
            if (adv.House.Comments != null)
            {
                 Comments = adv.House.Comments
                    .Select(r => new
                    {
                        r.Tenant.Firstname,
                        r.Tenant.Lastname,
                        r.CommentDate,
                        r.CommentDetails,
                    })
                    .ToList();
            }

            var HouseMainInfo = new
            {
                HouseName = adv.HouseName,
                HouseDescription = adv.houseDescription,
                RentalType = adv.rentalType,

                adv.House.governorate,
                adv.House.city,
                adv.House.street,
                HouseAverageRate = adv.House.AvrageRate,
                NumOfRaters = adv.House.NumOfRaters,
                ShowEstimatedPrice = (user != null && user.PremiumEnd >=DateTime.UtcNow)
            };

            return Ok(new { HouseMainInfo , HouseInspectionInfo , RentalUnitInfo , Comments });
        }
        [HttpGet("MyRental")]
        public async Task<IActionResult>GetRentalUnit()
        {
            var Id = tokenGenertor.GetCurUser();
            var rental = await apiContext.Rentals.Where(r=>r.TenantUsername==Id).ToListAsync();
            if (rental.Any())
                return Ok(rental);
            return NoContent();

        


        }
        [Authorize]
        [HttpPut("premium")]
        public async Task<IActionResult> UpgradeToPremium()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.FirstOrDefaultAsync(r => r.Username == username);
            if (user == null)
                return Unauthorized();

            if (user.PremiumEnd >= DateTime.UtcNow)
                return BadRequest(new { message = "You are already a premium user." });

            if (user.Balance < 1000)
                return BadRequest(new { message = "Insufficient balance to upgrade to premium." });

            user.PremiumBegin = DateTime.UtcNow;
            user.PremiumEnd = DateTime.UtcNow.AddYears(1);
            user.Balance -= 1000;

            await apiContext.SaveChangesAsync();

            return Ok(new { message = "Successfully upgraded to premium." });


        }
    }

}
