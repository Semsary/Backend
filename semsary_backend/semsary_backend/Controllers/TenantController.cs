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
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return Unauthorized();

            if (user.UserType != Enums.UserType.Tenant)
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
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return Unauthorized();

            if (user.UserType != Enums.UserType.Tenant)
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
                await apiContext.Rates.AddAsync(rate);
                await apiContext.SaveChangesAsync();
            }
            else
            {
                pastRate.StarsNumber = rateDTO.StarsNumber;
                pastRate.RateDate = DateTime.UtcNow;
                apiContext.Rates.Update(pastRate);
                await apiContext.SaveChangesAsync();
            }
            return Ok(new { message = "Rate submitted successfully" });
        }

        [HttpPost("Add/Comment/{houseId}")]
        public async Task<IActionResult> AddComment(string houseId, [FromBody] CommentDTO rateDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return Unauthorized();

            if (user.UserType != Enums.UserType.Tenant)
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

        [HttpPost("Make/Rental/Request/")]
        public async Task<IActionResult> MakeRentalRequest(RentalDTO rentalDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

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

            if (rentalUnits.Count != rentalDTO.RentalUnitIds.Count || rentalDTO.RentalUnitIds.Count == 0)
                return BadRequest(new { message = "Please enter valid rental unit IDs." });

            string advId = rentalUnits[0].AdvertisementId;
            if (rentalUnits.Any(u => u.AdvertisementId != advId))
                return BadRequest(new { message = "All rental units must belong to the same advertisement." });

            DateTime currentDate = DateTime.UtcNow;
            if (rentalDTO.StartDate < currentDate || rentalDTO.EndDate < currentDate)
                return BadRequest(new { message = "The rental period must be in the future." });

            if (rentalDTO.StartDate > rentalDTO.EndDate)
                return BadRequest(new { message = "The beginning of rental period cannot be after its end." });

            var allRentals = rentalUnits.SelectMany(u => u.Rentals).ToList();

            bool hasConflict = allRentals.Any(rent =>
                 (rentalDTO.StartDate < rent.EndDate && rentalDTO.StartDate >= rent.StartDate) ||
                 (rentalDTO.EndDate > rent.StartDate && rentalDTO.EndDate <= rent.EndDate));

            if (hasConflict)
                return BadRequest(new { message = "This rental unit is already rented during the requested period." });

            if (rentalDTO.StartArrivalDate > rentalDTO.EndDate || rentalDTO.EndArrivalDate > rentalDTO.EndDate)
                return BadRequest(new { message = "Invalid arrival period, you cannot arrive after your rental period ends." });

            if (rentalDTO.StartArrivalDate < currentDate || rentalDTO.EndArrivalDate < currentDate)
                return BadRequest(new { message = "The arrival period must be in the future." });

            if (rentalDTO.StartArrivalDate > rentalDTO.EndArrivalDate)
                return BadRequest(new { message = "The beginning of arrival period cannot be after its end." });

            if (rentalDTO.RentalUnitIds == null || rentalDTO.RentalUnitIds.Count == 0)
                return BadRequest(new { message = "Rental units cannot be empty." });

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
            await apiContext.SaveChangesAsync();

            Landlord lanlord = house.owner;
            await notificationService.SendNotificationAsync("طلب تأجير جديد", $"لقد حصلت علي طلب تأجير جديد من {user.Firstname} {user.Lastname}\nللمزيد من التفاصيل قم بزيارة الموقع.", lanlord);

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
                await apiContext.SaveChangesAsync();
                await notificationService.SendNotificationAsync("تم الغاء طلب حجز", $"قام {user.Firstname} {user.Lastname}\nبإلغاء الحجز بعد الفترة المسموحة\n و قد تم تحويل فلوس الضمان لحسابك.", lanlord);
                return Ok(new { message = "Rental request cancelled successfully" });
            }
            if (rental.status == Enums.RentalStatus.Accepted && rental.ResponseDate.AddDays(2) >= DateTime.UtcNow)
            {
                user.Balance += (int)(rental.WarrantyMoney - rental.WarrantyMoney * Rental.OurPercentage);
                apiContext.Remove(rental);
                await apiContext.SaveChangesAsync();
                await notificationService.SendNotificationAsync("تم الغاء طلب حجز", $"قام {user.Firstname} {user.Lastname}\n بإلغاء الحجز خلال الفترة المسموحة\nتستطيع الآن القيام بعملية التأجير للآخرين في هذه الفترة", lanlord);
                return Ok(new { message = "Rental request cancelled successfully" });
            }
            return BadRequest();
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
                await notificationService.SendNotificationAsync("طلب تأكيد الوصول", $"قام {user.Firstname} {user.Lastname}\nبطلب تأكيد الوصول قم لتأكيد أو رفض العملية فم بزيارة الموقع", lanlord);
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

            int pageNumber = 1;
            int pageSize = 20;

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
                        Advertisements = h.Advertisements,
                        LatestInspection = h.HouseInspections
                            .OrderByDescending(i => i.InspectionDate)
                            .FirstOrDefault(),
                        gover= h.governorate
                    })
                    .ToListAsync();

                var sortedAds = houses
                    .OrderByDescending(h => recommendation.recommend(user, h.LatestInspection,h.gover))
                    .SelectMany(h => h.Advertisements)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return Ok(sortedAds);
            }
            else
            {
                var ads = await houseQuery
                    .SelectMany(h => h.Advertisements)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(ads);
            }
        }
    }
}
