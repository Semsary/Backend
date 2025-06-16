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

            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsBlocked == true)
                return BadRequest("You are blocked so you can't perform this operation");

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
            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }
            if (user.IsBlocked == true)
                return BadRequest("You are blocked so you can't perform this operation");

            var notInspectedHouses = await apiContext.Houses
                .Where(h => h.LandlordUsername == user.Username && (h.HouseInspections == null || h.HouseInspections.Count()==0))
                .Select(h => new
                {
                    h.HouseId,
                    h.governorate,
                    h.city,
                    h.street
                })
                .ToListAsync();

            var inspectedHouses = await apiContext.Houses
                .Where(h => h.LandlordUsername == user.Username && h.HouseInspections.Count()>0)
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
            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.IsBlocked == true)
                return BadRequest("You are blocked so you can't perform this operation");

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
        [HttpGet("inspection/get/{HouseId}")]
        public async Task<IActionResult> GetInspection(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
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
                .Where(i => i.HouseId == HouseId && i.inspectionStatus == Enums.InspectionStatus.Completed)
                .OrderByDescending(i => i.InspectionRequestDate)
                .FirstOrDefaultAsync();

            if (lastInspection == null)
            {
                return NotFound(new { message = "No Completed inspections found for this house" });
            }

            return Ok(lastInspection);
        }

        [HttpPut("inspection/approve/{HouseId}")]
        public async Task<IActionResult> inspectionApprove(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
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
                .Where(i => i.HouseId == HouseId && i.inspectionStatus == Enums.InspectionStatus.Completed)
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
            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
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
            if(user.IsBlocked == true)
                return BadRequest("You are blocked so you can't perform this operation");
            var house = apiContext.Houses.Include(h => h.owner).Include(h => h.Advertisements).Where(h => h.HouseId == dto.HouseId).FirstOrDefault();
            if (house == null)
                return NotFound();
            if (house.owner.Username != username)
                return Forbid();
            if(house.owner.Balance < 5000)
                return BadRequest("You don't have enough balance to create an advertisement, you need at least 5000 EGP");

            var LastInspiction = apiContext.HouseInspections.Where(hin => hin.HouseId == dto.HouseId && hin.inspectionStatus == Enums.InspectionStatus.Aproved).OrderByDescending(t => t.InspectionDate).FirstOrDefault();
            if (LastInspiction == null)
                return BadRequest("house must have at least one inspiction");

            if(!(house.Advertisements == null || house.Advertisements.Count == 0))
                return BadRequest("This house already has an advertisement");

            house.owner.Balance -= 5000;
            var adv = new Advertisement()
            {
                AdvertisementId = Ulid.NewUlid().ToString(),
                HouseId = dto.HouseId,
                PublishDate = DateTime.Now,
                HouseName = dto.HouseName,
                houseDescription = dto.HouseDescription,
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
            adv.rentalType = dto.RentalType;
            apiContext.Advertisements.Add(adv);
            apiContext.SaveChanges();
            return Created();
        }
        [HttpGet("get/All/Advertisements")]
        public async Task<IActionResult> GetAllAdvertisements()
        {
            var username = tokenHandler.GetCurUser();
            var user = await apiContext.Landlords.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            var advertisements = await apiContext.Advertisements
                .Include(a => a.House)
                .Include(a => a.RentalUnits)
                .ToListAsync();

            if (advertisements == null || advertisements.Count == 0)
            {
                return NotFound(new { message = "No advertisements found" });
            }

            return Ok(advertisements);
        }
        [HttpDelete("delete/Advertisement/{advId}")]
        public async Task<IActionResult> deleteAdv(string advId)
        {
            var username = tokenHandler.GetCurUser();
            var user = await apiContext.Landlords.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

            var adv = await apiContext.Advertisements.Where(r => r.AdvertisementId == advId).FirstOrDefaultAsync();
            if(adv == null)
                return NotFound("Advertisement not found");         

            var rentalUnits = await apiContext.RentalUnits
                .Where(r => r.AdvertisementId == advId)
                .ToListAsync();

            apiContext.RentalUnits.RemoveRange(rentalUnits);
            apiContext.Advertisements.Remove(adv);
            await apiContext.SaveChangesAsync();
            return Ok(new { message = "Advertisement deleted successfully" });
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

            if (user.IsBlocked == true)
                return BadRequest("You are blocked so you can't perform this operation");

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
        [HttpGet("get/all/approved/houses")]
        public async Task<IActionResult> GetAllApprovedHouses()
        {
            var userid = tokenHandler.GetCurUser();
            var user = apiContext.Landlords.FirstOrDefault(x => x.Username == userid);
            if (user == null)
            {
                return Unauthorized();
            }

            var result = await apiContext.Houses
                 .Where(h => h.LandlordUsername == user.Username)
                 .Select(h => new
                 {
                     House = new
                     {
                         h.governorate,
                         h.city,
                         h.street,
                         estimated_Price = priceEstimator.EstimatePrice(h.HouseInspections.OrderByDescending(i => i.InspectionDate).FirstOrDefault(), h.governorate),
                         hasPublishedAdv = h.Advertisements.Any(a => a.HouseId == h.HouseId)
                     },
                     LastApprovedInspection = h.HouseInspections
                         .Where(i => i.inspectionStatus == Enums.InspectionStatus.Aproved)
                         .OrderByDescending(i => i.InspectionDate)
                         .FirstOrDefault()
                 })
                 .Where(x => x.LastApprovedInspection != null)
                 .ToListAsync();
            return Ok(result);
        }

    }
}
//w s