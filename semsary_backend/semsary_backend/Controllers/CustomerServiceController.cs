using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Models;
using semsary_backend.Service;

namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerServiceController : ControllerBase
    {
        private readonly TokenService tokenGenertor;
        private readonly ApiContext apiContext;
        private readonly R2StorageService r2StorageService;

        public CustomerServiceController(TokenService TokenGenertor, ApiContext apiContext,R2StorageService r2StorageService)
        {
            tokenGenertor = TokenGenertor;
            this.apiContext = apiContext;
            this.r2StorageService = r2StorageService;
        }

        [HttpPost("HouseInspection/{houseId}")]
        public async Task<IActionResult> MakeHouseInspection(string houseId, [FromBody] HouseInspectionDTO HouseInspectionDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid();//update in case of the token isn't valid or not Customerservice ,return forbidden
            }
            if (HouseInspectionDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var house = await apiContext.Houses.FindAsync(houseId);
            if (house == null)
                return NotFound("House not found");

            var inspection = new HouseInspection
            {
                HouseInspectionId = Guid.NewGuid().ToString(),  // unique random value
                HouseId = houseId,
                InspectorId = user.Username,
                InspectionDate = DateTime.UtcNow,
                inspectionStatus = InspectionStatus.Aproved,

                FloorNumber = HouseInspectionDTO.FloorNumber,
                NumberOfAirConditionnar = HouseInspectionDTO.NumberOfAirConditionnar,
                NumberOfPathRooms = HouseInspectionDTO.NumberOfPathRooms,
                NumberOfBedRooms = HouseInspectionDTO.NumberOfBedRooms,
                NumberOfBeds = HouseInspectionDTO.NumberOfBeds,
                NumberOfBalacons = HouseInspectionDTO.NumberOfBalacons,
                NumberOfTables = HouseInspectionDTO.NumberOfTables,
                NumberOfChairs = HouseInspectionDTO.NumberOfChairs,
                HouseFeature=HouseInspectionDTO.HouseFeature,
            };
            foreach(var img in HouseInspectionDTO.HouseImages)
            {
                var url= await r2StorageService.UploadFileAsync(img);
                inspection.HouseImages.Add(url);

            }
            
            apiContext.HouseInspections.Add(inspection);
            await apiContext.SaveChangesAsync();

            return Ok($"House Inespection created successfully with id {inspection.HouseInspectionId}");
        }

        [HttpPut("Inspection/acknowledge/{houseInspectionId}")]
        public async Task<IActionResult> EditInspectionStatus(string houseInspectionId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid("User not found.");
            }
            
            
            var houseInspection = await apiContext.FindAsync<HouseInspection>(houseInspectionId);
            if (houseInspection == null)
                return NotFound("House inspection not found");

            houseInspection.inspectionStatus = InspectionStatus.InProgress;
            houseInspection.InspectorId = user.Username;

            await apiContext.SaveChangesAsync();

            return Ok($"House inspection status updated to \"{InspectionStatus.InProgress}\" successfully");
        }

        [HttpGet("ShowComplaint/{complaintId}")]
        public async Task<IActionResult> GetComplaint(string complaintId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return NotFound("User not found.");
            }

            int id;
            if (!int.TryParse(complaintId, out id))
            {
                return BadRequest("Invalid data format.");
            }

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
                return NotFound("Complaint not found");

            ComplaintRequestDTO complaintDTO = new ComplaintRequestDTO
            {
                ComplaintDetails = complaint.ComplaintDetails,
                SubmittedBy = complaint.SubmittedBy,
                SubmittingDate = complaint.SubmittingDate,
                RentalId = complaint.RentalId,
            };
            return Ok(complaintDTO);
        }
        [HttpPut("Complaint/acknowledge/{complaintId}")]
        public async Task<IActionResult> ComplaintStatus(string complaintId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid();
            }

            
            int id;
            if (!int.TryParse(complaintId, out id))
            {
                return BadRequest("Invalid data format.");
            }

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
                return NotFound("Complaint not found");

            complaint.status = ComplainStatus.Bending;
            complaint.VerifiedBy = user.Username;
            await apiContext.SaveChangesAsync();

            return Ok($"Complaint status updated to \"{complaint.status}\" successfully");
        }
        [HttpPost("ReviewComplaint/{complaintId}")]
        public async Task<IActionResult> ReviewComplaint(string complaintId , [FromBody] ComplaintReviewDTO complaintReviewDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return NotFound("User not found.");
            }
            int id;
            if (!int.TryParse(complaintId, out id))
            {
                return BadRequest("Invalid data format.");
            }

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
            {
                return NotFound("Complaint not found");
            }
            complaint.VerifiedBy = user.Username;
            complaint.ComplaintReview = complaintReviewDTO.ComplaintReview;
            complaint.ReviewDate = DateTime.UtcNow;

            return Ok("Complaint review added successfully");
           
        }
    }
}
