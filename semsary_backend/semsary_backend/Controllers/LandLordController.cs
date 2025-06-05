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
namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LandLordController(TokenService tokenHandler,ApiContext apiContext) : ControllerBase
    {
        [HttpPost("create/house")]
        public async Task<IActionResult> CreateHouse([FromBody]  HouseDTO houseDTO)
        {
            var userid = tokenHandler.GetCurUser();
            
            var user=apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if(user == null)
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
        [HttpPost("inspection/request/{HouseId}")]
        public async Task<IActionResult> requestInspection(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user=apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if(user == null)
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

                return Forbid( "You are not the owner of this house");
            }
            var landlord=(Landlord)user;
            if(landlord.Balance<50)
            {
                return BadRequest(new { message = "You don't have enough balance to request an inspection" });
            }
            landlord.Balance -= 50;

            var inspection = new HouseInspection
            {
                HouseId = house.HouseId,
                InspectionRequestDate= DateTime.UtcNow,
            };
            
            await apiContext.HouseInspections.AddAsync(inspection);
            await apiContext.SaveChangesAsync();
            
            return Ok(new { message = "Inspection requested successfully" });
        }
        [HttpGet("get/house/{HouseId}")]
        public async Task<IActionResult> GetHouse(string HouseId)
        {
            var userid = tokenHandler.GetCurUser();
            var user=apiContext.SermsaryUsers.FirstOrDefault(x => x.Username == userid);
            if(user == null)
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
                .Include(h=>h.Advertisements)
                .FirstOrDefaultAsync(h => h.HouseId == HouseId);
            
            if (house == null)
            {
                return NotFound(new { message = "House not found" });
            }
            if(house.LandlordUsername != user.Username)
            {
                return Forbid( "You are not the owner of this house");
            }

            return Ok(house);
        }
        [Authorize]
        [HttpPost("CreateAdvertisement")]
        public async Task<IActionResult> CreateAdv(AdvDTO dto)
        {
            var username=tokenHandler.GetCurUser();
            var user= await apiContext.Landlords.Where(r=>r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();
            var house = apiContext.Houses.Include(h => h.owner).Where(h => h.HouseId == dto.HouseId).FirstOrDefault();
            if (house == null)
                return NotFound();
            if (house.owner.Username != username)
                return Forbid();
            var LastInspiction=apiContext.HouseInspections.Where(hin=>hin.HouseId == dto.HouseId && hin.inspectionStatus== Enums.InspectionStatus.Aproved).OrderByDescending(t=>t.InspectionDate).FirstOrDefault();
            if (LastInspiction == null)
                return BadRequest("house must have at least one inspiction");

            var adv = new Advertisement()
            {
                AdvertisementId = Ulid.NewUlid().ToString(),
                HouseId = dto.HouseId,
                PublishDate = DateTime.Now,
                RentalUnits = new()

            };

            if(dto.RentalType==Enums.RentalType.ByHouse)
            {
                var rentunit = new RentalUnit();
                rentunit.RentalUnitId = Ulid.NewUlid().ToString();
                rentunit.AdvertisementId=adv.AdvertisementId;
                rentunit.MonthlyCost=dto.MonthlyCost;
                rentunit.DailyCost=dto.DailyCost;
                rentunit.Advertisement = adv;
                adv.RentalUnits.Add(rentunit);
                
            }
            else
            {
                for(int i=1;i<= LastInspiction.NumberOfBeds;i++)
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
        

    }
}
//w s