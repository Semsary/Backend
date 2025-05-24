using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Esf;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Models;
using semsary_backend.Service;

namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantController(TokenService tokenGenertor, ApiContext apiContext , NotificationService notificationService) : ControllerBase
    {
        [HttpPost("MakeComplaint/{rentalId}")]
        public async Task<IActionResult> MakeComplaint(string rentalId, [FromBody] ComplaintRequestForTentatDTO complaintRequestDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if(user ==null)
            {
                return Unauthorized();
            }   
            if (user.UserType != Enums.UserType.Tenant)
            {
                return Forbid();
            }
            if (complaintRequestDTO == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            int id;
            if(!int.TryParse(rentalId , out id))
            {
                return BadRequest(new { message = "Invalid data format." });
            }

            var rental = await apiContext.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound(new { message = "There is no rental found with this id" });
            }
            if (rental.TenantUsername != user.Username)
            {
                return Forbid();
            }

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

        [HttpPost("SetRate/{houseId}")]
        public async Task<IActionResult> SetRate(string houseId , [FromBody] RateDTO rateDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.Tenant)
            {
                return Forbid();
            }
            if (rateDTO == null)
            {
                return BadRequest(new { message = "Invalid data." });
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var house = await apiContext.Houses
                .Include(r => r.Rentals)
                .FirstOrDefaultAsync(h => h.HouseId == houseId);
            if (house == null)
            {
                return NotFound(new { message = "There is no house found with this id" });
            }
            var rental = house.Rentals.FirstOrDefault(r => r.TenantUsername == user.Username);
            if(rental == null)
            {
                return Forbid();
            }
            var rate = new Rate
            {
                HouseId = houseId,
                TenantUsername = user.Username,
                RateDate = DateTime.UtcNow,
                StarsNumber = rateDTO.StarsNumber,
                RateDetails = rateDTO.RateDetails,
            };
            await apiContext.Rates.AddAsync(rate);
            await apiContext.SaveChangesAsync();
            return Ok(new { message = "Rate submitted successfully" });
        }

        // temporary function to check notifications, it is not the final version of MakeRentalRequest function
        [HttpPost("MakeRentalRequest/{houseId}")]
        public async Task<IActionResult> MakeRentalRequest(string houseId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
            {
                return Unauthorized();
            }
            if (user.UserType != Enums.UserType.Tenant)
            {
                return Forbid();
            }
            var house = await apiContext.Houses.Include(h => h.owner).FirstOrDefaultAsync(h => h.HouseId == houseId);
            if (house == null)
            {
                return NotFound(new { message = "There is no house found with this id" });
            }
            Landlord lanlord = house.owner;
            await notificationService.SendNotificationAsync("test", "first message", lanlord);
            return Ok(new { message = "Rental request submitted successfully" });
        }
    }
}
