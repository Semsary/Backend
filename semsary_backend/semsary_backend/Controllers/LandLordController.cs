using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Service;
using semsary_backend.Models;
using Microsoft.EntityFrameworkCore;
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
              owner=(Landlord)user

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
        

    }
}
