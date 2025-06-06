using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Esf;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Models;
using semsary_backend.Service;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost("Make/Rental/Request/")]
        public async Task<IActionResult> MakeRentalRequest(RentalDTO rentalDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.Tenant.Where(r => r.Username == username).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized();

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
            
            if (rentalDTO.StartDate > rentalDTO.EndDate )
                return BadRequest(new { message = "The beginning of rental period cannot be after its end." });

            var allRentals = rentalUnits.SelectMany(u => u.Rentals).ToList();

            bool hasConflict = allRentals.Any(rent =>
                 (rentalDTO.StartDate < rent.EndDate && rentalDTO.StartDate >= rent.StartDate) ||
                 (rentalDTO.EndDate > rent.StartDate && rentalDTO.EndDate <= rent.EndDate));

            if (hasConflict)
                return BadRequest(new { message = "This rental unit is already rented during the requested period." });
            
            if (rentalDTO.StartArrivalDate > rentalDTO.EndDate || rentalDTO.EndArrivalDate > rentalDTO.EndDate)
                return BadRequest(new { message = "Invalid arrival period, you cannot arrive after your rental period ends."});

            if(rentalDTO.StartArrivalDate < currentDate || rentalDTO.EndArrivalDate < currentDate )
                return BadRequest(new { message = "The arrival period must be in the future." });

            if (rentalDTO.StartArrivalDate > rentalDTO.EndArrivalDate)
                return BadRequest(new { message = "The beginning of arrival period cannot be after its end." });

            if(rentalDTO.RentalUnitIds == null || rentalDTO.RentalUnitIds.Count == 0)
                return BadRequest(new { message = "Rental units cannot be empty." });


            var rental = new Rental
            {
                StartDate = rentalDTO.StartDate,
                EndDate = rentalDTO.EndDate,
                StartArrivalDate = rentalDTO.StartArrivalDate,
                EndArrivalDate = rentalDTO.EndArrivalDate,
                HouseId = rentalDTO.HouseId,
                RentalType = rentalDTO.RentalType,
                RentalUnitIds = rentalDTO.RentalUnitIds,
                TenantUsername = user.Username,
                CreationDate = DateTime.UtcNow,
                status = Enums.RentalStatus.Bending
            };

            await apiContext.Rentals.AddAsync(rental);
            await apiContext.SaveChangesAsync();

            Landlord lanlord = house.owner;
            await notificationService.SendNotificationAsync("New Rental Request", "There is a new ", lanlord);
            return Ok(rental.RentalId);

        }

        // temporary function to check notifications
        [HttpPost("TestNotifications/{houseId}")]
        public async Task<IActionResult> MakeRentalRequestt(string houseId)
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
