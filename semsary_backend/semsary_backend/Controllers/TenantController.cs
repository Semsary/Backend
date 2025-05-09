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
    public class TenantController : ControllerBase
    {
        private readonly TokenService tokenGenertor;
        private readonly ApiContext apiContext;
        private readonly R2StorageService r2StorageService;

        public TenantController(TokenService TokenGenertor, ApiContext apiContext, R2StorageService r2StorageService)
        {
            tokenGenertor = TokenGenertor;
            this.apiContext = apiContext;
            this.r2StorageService = r2StorageService;
        }

        [HttpPost("MakeComplaint/{rentalId}")]
        public async Task<IActionResult> MakeComplaint(string rentalId, [FromBody] ComplaintRequestForTentatDTO complaintRequestDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != Enums.UserType.Tenant)
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
                return Forbid("You are not the current tenant of this house" );
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

    }
}
