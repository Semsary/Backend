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
        public CustomerServiceController(TokenService TokenGenertor, ApiContext apiContext)
        {
            tokenGenertor = TokenGenertor;
            this.apiContext = apiContext;
        }

        [HttpPost("HouseInspection/{houseId}")]
        public async Task<IActionResult> MakeHouseInspection(string houseId, [FromBody] HouseInspectionDTO HouseInspectionDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return NotFound("User not found.");
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

                HaveNearHospital = HouseInspectionDTO.HaveNearHospital,
                HaveNearGym = HouseInspectionDTO.HaveNearGym,
                HaveNearPlayGround = HouseInspectionDTO.HaveNearPlayGround,
                HaveNearSchool = HouseInspectionDTO.HaveNearSchool,
                HaveNearUniversity = HouseInspectionDTO.HaveNearUniversity,
                HaveNearSupermarket = HouseInspectionDTO.HaveNearSupermarket,
                HaveNearRestaurant = HouseInspectionDTO.HaveNearRestaurant,
                HaveNearBusStation = HouseInspectionDTO.HaveNearBusStation,
                HaveNearBank = HouseInspectionDTO.HaveNearBank,

                HaveWiFi = HouseInspectionDTO.HaveWiFi,
                HaveTV = HouseInspectionDTO.HaveTV,
                Havekitchen = HouseInspectionDTO.Havekitchen,
                HaveElevator = HouseInspectionDTO.HaveElevator,
                HaveWashingMachine = HouseInspectionDTO.HaveWashingMachine,
                HaveCooker = HouseInspectionDTO.HaveCooker,
                HaveFridge = HouseInspectionDTO.HaveFridge,
                HaveHeater = HouseInspectionDTO.HaveHeater,
                HaveInternet = HouseInspectionDTO.HaveInternet,
                HaveSalon = HouseInspectionDTO.HaveSalon,
                DiningRoom = HouseInspectionDTO.DiningRoom,
                HouseImages = HouseInspectionDTO.HouseImages
            };
            foreach (var img in inspection.HouseImages)
            {
                img.HouseInspectionId = inspection.HouseInspectionId;
            }
            apiContext.HouseInspections.Add(inspection);
            await apiContext.SaveChangesAsync();

            return Ok($"House Inespection created successfully with id {inspection.HouseInspectionId}");
        }

        [HttpPut("InspectionStatus/{houseInspectionId}")]
        public async Task<IActionResult> EditInspectionStatus(string houseInspectionId, [FromBody] InspectionStatusDTO inspectionStatusDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return NotFound("User not found.");
            }
            if (inspectionStatusDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var houseInspection = await apiContext.FindAsync<HouseInspection>(houseInspectionId);
            if (houseInspection == null)
                return NotFound("House inspection not found");

            houseInspection.inspectionStatus = inspectionStatusDTO.inspectionStatus;
            await apiContext.SaveChangesAsync();

            return Ok($"House inspection status updated to \"{inspectionStatusDTO.inspectionStatus}\" successfully");
        }

        [HttpGet("Complaint/{complaintId}")]
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
        [HttpPut("ComplaintStatus/{houseInspectionId}")]
        public async Task<IActionResult> ComplaintStatus(string complaintId, [FromBody] ComplaintStatusDTO complaintStatusDTO)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return NotFound("User not found.");
            }
            if (complaintStatusDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            int id;
            if (!int.TryParse(complaintId, out id))
            {
                return BadRequest("Invalid data format.");
            }

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
                return NotFound("Complaint not found");

            complaint.status = complaintStatusDTO.status;
            await apiContext.SaveChangesAsync();

            return Ok($"Complaint status updated to \"{complaint.status}\" successfully");
        }
        [HttpPost("ReviewInspection/{complaintId}")]
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
            complaint.VerifiedBy = user as CustomerService;
            complaint.ComplaintReview = complaintReviewDTO.ComplaintReview;
            complaint.ReviewDate = DateTime.UtcNow;

            return Ok("Complaint review added successfully");
           
        }
    }
}
