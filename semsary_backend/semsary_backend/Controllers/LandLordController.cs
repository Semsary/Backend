using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Service;
using semsary_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using semsary_backend.Enums;
namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LandLordController(TokenService tokenHandler, ApiContext apiContext, NotificationService notificationService,PriceEstimator priceEstimator) : ControllerBase
    {
        [HttpPost("create/house")]
        public async Task<IActionResult> CreateHouse([FromBody] HouseDTO houseDTO)
        {
            var userid = tokenHandler.GetCurUser();

            var user = apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.landlord)
            {
                return Forbid();
            }

            var house = new House
            {
                governorate = houseDTO.address._gover,
                city = houseDTO.address._city,
                street = houseDTO.address.street,

                LandlordUsername = user.Username,
                HouseId = Ulid.NewUlid().ToString(),
                owner = (Landlord)user,
                HouseInspections = new()

            };
            await apiContext.Houses.AddAsync(house);
            await apiContext.SaveChangesAsync();
            return Ok(new { message = "House created successfully", houseId = house.HouseId });

        }

        [HttpGet("Houses/GetAll")]
        public async Task<IActionResult> GetAllHouses()
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.landlord)
            {
                return Forbid();
            }
            var notInspectedHouses = await apiContext.Houses
                .Where(h => h.LandlordUsername == user.Username && (h.HouseInspections == null || !h.HouseInspections.Any()))
                .Select(h => new
                {
                    h.HouseId,
                    h.governorate,
                    h.city,
                    h.street
                })
                .ToListAsync();

            var inspectedHouses = await apiContext.Houses
                .Where(h => h.LandlordUsername == user.Username && h.HouseInspections.Any())
                .Select(h => new
                {
                    h.HouseId,
                    h.governorate,
                    h.city,
                    h.street,
                    LastInspectionStatus = h.HouseInspections
                        .OrderByDescending(i => i.InspectionDate)
                        .Select(i => i.inspectionStatus)
                        .FirstOrDefault(),
                    estimated_Price= priceEstimator.EstimatePrice(h.HouseInspections.OrderByDescending(i => i.InspectionDate).FirstOrDefault(),h.governorate)
                })
                .ToListAsync();

            return Ok(new { notInspectedHouses, inspectedHouses });
        }

        [HttpPost("inspection/request/{HouseId}")]
        public async Task<IActionResult> requestInspection(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.landlord)
            {
                return Forbid();
            }

            var house = await apiContext.Houses.FindAsync(HouseId);
            if (house == null)
            {
                return NotFound(new { message = "House not found" });
            }
            if (house.LandlordUsername != user.Username)
            {

                return Forbid("You are not the owner of this house");
            }
            var landlord = (Landlord)user;
            if (landlord.Balance < 50)
            {
                return BadRequest(new { message = "You don't have enough balance to request an inspection" });
            }
            landlord.Balance -= 50;

            var inspection = new HouseInspection
            {
                HouseId = house.HouseId,
                InspectionRequestDate = DateTime.UtcNow,
            };

            await apiContext.HouseInspections.AddAsync(inspection);
            await apiContext.SaveChangesAsync();

            return Ok(new { message = "Inspection requested successfully" });
        }

        [HttpPut("inspection/approve/{HouseId}")]
        public async Task<IActionResult> inspectionApprove(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.landlord)
            {
                return Forbid();
            }
            var house = await apiContext.Houses.FindAsync(HouseId);
            if (house == null)
            {
                return NotFound(new { message = "House not found" });
            }
            if (house.LandlordUsername != user.Username)
            {
                return Forbid("You are not the owner of this house");
            }

            var lastInspection = await apiContext.HouseInspections
                .Where(i => i.HouseId == HouseId && i.inspectionStatus == Enums.InspectionStatus.InProgress)
                .OrderByDescending(i => i.InspectionDate)
                .FirstOrDefaultAsync();

            if (lastInspection == null)
                return NotFound(new { message = "No inspections need to be approved found for this house" });

            lastInspection.inspectionStatus = Enums.InspectionStatus.Aproved;
            await apiContext.SaveChangesAsync();

            return Ok(new { message = "Inspection approved successfully" });
        }

        [HttpGet("get/house/{HouseId}")]
        public async Task<IActionResult> GetHouse(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.landlord)
            {
                return Forbid();
            }

            var house = await apiContext.Houses
                .Include(h => h.Rates)
                .Include(h => h.HouseInspections)
                .Include(h => h.Advertisements)
                .FirstOrDefaultAsync(h => h.HouseId == HouseId);

            if (house == null)
            {
                return NotFound(new { message = "House not found" });
            }
            if (house.LandlordUsername != user.Username)
            {
                return Forbid("You are not the owner of this house");
            }

            return Ok(house);
        }
        [Authorize]
        [HttpPost("CreateAdvertisement")]
        public async Task<IActionResult> CreateAdv(AdvDTO dto)
        {
            var username = tokenHandler.GetCurUser();
            var user = await apiContext.Landlords.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();
            var house = apiContext.Houses.Include(h => h.owner).Where(h => h.HouseId == dto.HouseId).FirstOrDefault();
            if (house == null)
                return NotFound();
            if (house.owner.Username != username)
                return Forbid();
            var LastInspiction = apiContext.HouseInspections.Where(hin => hin.HouseId == dto.HouseId && hin.inspectionStatus == Enums.InspectionStatus.Aproved).OrderByDescending(t => t.InspectionDate).FirstOrDefault();
            if (LastInspiction == null)
                return BadRequest("house must have at least one inspiction");

            var adv = new Advertisement()
            {
                AdvertisementId = Ulid.NewUlid().ToString(),
                HouseId = dto.HouseId,
                PublishDate = DateTime.Now,
                RentalUnits = new()

            };

            if (dto.RentalType == Enums.RentalType.ByHouse)
            {
                var rentunit = new RentalUnit();
                rentunit.RentalUnitId = Ulid.NewUlid().ToString();
                rentunit.AdvertisementId = adv.AdvertisementId;
                rentunit.MonthlyCost = dto.MonthlyCost;
                rentunit.DailyCost = dto.DailyCost;
                rentunit.Advertisement = adv;
                adv.RentalUnits.Add(rentunit);

            }
            else
            {
                for (int i = 1; i <= LastInspiction.NumberOfBeds; i++)
                {
                    var rentunit = new RentalUnit();

                    rentunit.RentalUnitId = Ulid.NewUlid().ToString();
                    rentunit.AdvertisementId = adv.AdvertisementId;
                    rentunit.MonthlyCost = dto.MonthlyCost;
                    rentunit.DailyCost = dto.DailyCost;
                    rentunit.Advertisement = adv;
                    adv.RentalUnits.Add(rentunit);
                }
            }
            apiContext.Advertisements.Add(adv);
            apiContext.SaveChanges();
            return Created();
        }
        [Authorize]
        [HttpGet("rental/request")]
        public async Task<IActionResult> GetRentalRequests()
        {
            var username = tokenHandler.GetCurUser();
            var user = await apiContext.Landlords.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();
            var requests = await apiContext.Rentals.Include(r => r.RentalUnit).ThenInclude(r => r.Advertisement).ThenInclude(a => a.House)
                .Where(r => r.RentalUnit[0].Advertisement.House.owner.Username == username && r.status == Enums.RentalStatus.Bending &&r.WarrantyMoney<=r.Tenant.Balance)
                .ToListAsync();
            return Ok(requests);





        }
        [Authorize]
        [HttpPut("rentalrequest/{RentalId}")]
        public async Task<IActionResult> reviewRentalRequest(int RentalId, RentalStatus status)
        {
            var username = tokenHandler.GetCurUser();
            var user = await apiContext.Landlords.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            var rental = await apiContext.Rentals.Include(r => r.RentalUnit).ThenInclude(r => r.Advertisement).ThenInclude(a => a.House)
                .Where(r => r.RentalId == RentalId && r.RentalUnit[0].Advertisement.House.owner.Username == username && r.status == Enums.RentalStatus.Bending)
                .FirstOrDefaultAsync();
            if (rental == null)
                return NotFound();
            if (status == RentalStatus.Accepted)
            {
                rental.status = Enums.RentalStatus.Accepted;
                var message = "لقد تم الموافقة على طلب الايجار خاصتك و تم خصم التامين من رصيدك";
                var title = "تم الموافقة على طلب الايجار";
                var tenant = await apiContext.Tenant.Include(t => t.DeviceTokens).FirstOrDefaultAsync(r => r.Username == rental.TenantUsername);
                if (tenant == null)
                {
                    return NotFound("Tenant not found");
                }

                if (tenant.Balance< rental.WarrantyMoney)
                {
                    rental.status= Enums.RentalStatus.Rejected;
                    var message2 = "لقد تم رفض طلب الايجار خاصتك بسبب عدم كفاية الرصيد, يرجى المحاولة مرة اخرى لاحقا";
                    var title2 = "رفض طلب الايجار";
                    await notificationService.SendNotificationAsync(title2, message2, tenant);
                    apiContext.SaveChanges();
                    return BadRequest("Tenant does not have enough balance to accept the rental request");
                }

                tenant.Balance -= rental.WarrantyMoney;

                await notificationService.SendNotificationAsync(title, message, tenant);
                apiContext.SaveChanges();
                return Ok(new { message = "Rental request accepted successfully" });



            }
            else if (status == RentalStatus.Rejected)
            {
                rental.status = Enums.RentalStatus.Rejected;
                var message = "لقد تم رفض طلب الايجار خاصتك, لمزيد من التفاصيل عن سبب الرفض تواصل مع المؤجر ";
                var title = "رفض طلب الايجار";
                var tenant = await apiContext.Tenant.Include(t => t.DeviceTokens).FirstOrDefaultAsync(r => r.Username == rental.TenantUsername);
                await notificationService.SendNotificationAsync(title, message, tenant);
                apiContext.SaveChanges();
                return Ok(new { message = "Rental request rejected successfully" });
            }
            else
            {
                return BadRequest("Invalid status");
            }


        }
        [Authorize]
        [HttpPut("arrivalrequest/{RentalId}")]
        public async Task<IActionResult> ReviewArrivalRequest(int RentalId,RentalStatus rentalStatus)
        {
            var username = tokenHandler.GetCurUser();
            var user = await apiContext.Landlords.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            var rental = await apiContext.Rentals.Include(r => r.RentalUnit).ThenInclude(r => r.Advertisement).ThenInclude(a => a.House)
                .Where(r => r.RentalId == RentalId && r.RentalUnit[0].Advertisement.House.owner.Username == username && r.status == Enums.RentalStatus.ArrivalRequest)
                .FirstOrDefaultAsync();
            if (rental == null)
                return NotFound();
            var tenant= await apiContext.Tenant.Include(t => t.DeviceTokens).FirstOrDefaultAsync(r => r.Username == rental.TenantUsername);
            if (rentalStatus == Enums.RentalStatus.ArrivalAccept)
            {
                tenant.Balance += (int)(.95 * rental.WarrantyMoney);
                var message = "لقد تم الموافقة على طلب الوصول خاصتك, و تم اضافة التامين الى رصيدك";
                var title = "تم الموافقة على طلب الوصول";
                await notificationService.SendNotificationAsync(title, message, tenant);
                rental.status = Enums.RentalStatus.ArrivalAccept;
                apiContext.SaveChanges();
                return Ok(new { message = "Arrival request approved successfully" });
            }
            else if (rentalStatus == Enums.RentalStatus.ArrivalReject)
            {
                var message = "لقد تم رفض طلب الوصول خاصتك, لمزيد من التفاصيل عن سبب الرفض تواصل مع المؤجر ";
                var title = "رفض طلب الوصول";
                await notificationService.SendNotificationAsync(title, message, tenant);
            }
            
                return BadRequest("Invalid status");
            
        }
        
    }
}
//w s