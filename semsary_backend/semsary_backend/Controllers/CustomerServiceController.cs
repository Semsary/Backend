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
    public class CustomerServiceController(TokenService tokenGenertor, ApiContext apiContext, R2StorageService r2StorageService) : ControllerBase
    {
        [HttpPost("HouseInspection/create/{houseId}")]
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
                return BadRequest(new { message = "Invalid data." });
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var house = await apiContext.Houses.FindAsync(houseId);
            if (house == null)
                return NotFound(new { message = "House not found" });

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

            return Ok(new { message = $"House Inespection created successfully with id {inspection.HouseInspectionId}" });
        }

        [HttpPut("HouseInspection/acknowledge/{houseInspectionId}")]
        public async Task<IActionResult> EditInspectionStatus(string houseInspectionId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid();
            }
            
            
            var houseInspection = await apiContext.FindAsync<HouseInspection>(houseInspectionId);
            if (houseInspection == null)
                return NotFound(new { message = "House inspection not found" });

            houseInspection.inspectionStatus = InspectionStatus.InProgress;
            houseInspection.InspectorId = user.Username;

            await apiContext.SaveChangesAsync();

            return Ok(new { message = $"House inspection status updated to \"{InspectionStatus.InProgress}\" successfully" });
        }

        [HttpGet("Complaint/GetAll")]
        public async Task<IActionResult> GetComplaint()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid();
            }

            var complaints = await apiContext.Complaints.Where(c => c.status == ComplainStatus.Bending)
                .Select(c => new 
                {
                    ownerFirstName = c.Tenant.Firstname,
                    ownerLastName = c.Tenant.Lastname,
                    ComplaintId = c.ComplaintId,
                    ComplaintDetails = c.ComplaintDetails,
                    SubmittedBy = c.SubmittedBy,
                    SubmittingDate = c.SubmittingDate,
                    RentalId = c.RentalId,

                })
                .ToListAsync();

            return Ok(complaints);
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
                return BadRequest(new { message = "Invalid data format." });
            }

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
                return NotFound(new { message = "Complaint not found" });

            complaint.status = ComplainStatus.Bending;
            complaint.VerifiedBy = user.Username;
            await apiContext.SaveChangesAsync();

            return Ok(new { message = $"Complaint status updated to \"{complaint.status}\" successfully" });
        }
        [HttpPost("Complaint/Review/{complaintId}")]
        public async Task<IActionResult> ReviewComplaint(string complaintId , [FromBody] ComplaintReviewDTO complaintReviewDTO)
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
                return BadRequest(new { message = "Invalid data format." });
            }

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
            {
                return NotFound(new { message = "Complaint not found" });
            }
            complaint.VerifiedBy = user.Username;
            complaint.ComplaintReview = complaintReviewDTO.ComplaintReview;
            complaint.ReviewDate = DateTime.UtcNow;

            return Ok(new { message = "Complaint review added successfully" });
           
        }
        [HttpGet("HouseInspections/GetAll")]
        public async Task<IActionResult> GetHouseInspections()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid();
            }

            var houseInspections = await apiContext.HouseInspections
                .Include(h => h.House).Where(e=>e.inspectionStatus== InspectionStatus.Bending)
                .Select(h => new 
                {
                    HouseInspectionId = h.HouseInspectionId,
                    HouseId = h.HouseId,
                    ownerId = h.House.LandlordUsername,
                    ownerFirstName=h.House.owner.Firstname,
                    ownerLastName=h.House.owner.Lastname,
                    SubmitedAt=h.InspectionRequestDate,
                    Governorate=h.House.governorate,
                    city=h.House.city,
                    street=h.House.street

                })
                .ToListAsync();

            return Ok(houseInspections);
        }
    }
}
