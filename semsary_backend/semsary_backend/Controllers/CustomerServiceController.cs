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
    public class CustomerServiceController(TokenService tokenGenertor, ApiContext apiContext, R2StorageService r2StorageService, NotificationService notificationService) : ControllerBase
    {
        [HttpPut("HouseInspection/create/{houseId}")]
        public async Task<IActionResult> MakeHouseInspection(string houseId, [FromForm] HouseInspectionDTO HouseInspectionDTO)
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

            var inspection = await apiContext.HouseInspections
                .Where(i => i.HouseId == houseId && i.inspectionStatus == Enums.InspectionStatus.InProgress)
                .OrderByDescending(i => i.InspectionDate)
                .FirstOrDefaultAsync();

            if (inspection == null)
                return BadRequest(new { message = "There is no inspection in progress for this house." });

            var landlord = await apiContext.Landlords
                .FirstOrDefaultAsync(l => l.Username == house.LandlordUsername);

            if (landlord == null)
                return NotFound(new { message = "Landlord not found" });

            inspection.latitude = HouseInspectionDTO.latitude;
            inspection.longitude = HouseInspectionDTO.longitude;
            inspection.HouseInspectionId = Ulid.NewUlid().ToString();
            inspection.InspectorId = user.Username;
            inspection.InspectionDate = DateTime.UtcNow;
            inspection.inspectionStatus = InspectionStatus.Completed;
            inspection.FloorNumber = HouseInspectionDTO.FloorNumber;
            inspection.NumberOfAirConditionnar = HouseInspectionDTO.NumberOfAirConditionnar;
            inspection.NumberOfPathRooms = HouseInspectionDTO.NumberOfPathRooms;
            inspection.NumberOfBedRooms = HouseInspectionDTO.NumberOfBedRooms;
            inspection.NumberOfBeds = HouseInspectionDTO.NumberOfBeds;
            inspection.NumberOfBalacons = HouseInspectionDTO.NumberOfBalacons;
            inspection.NumberOfTables = HouseInspectionDTO.NumberOfTables;
            inspection.NumberOfChairs = HouseInspectionDTO.NumberOfChairs;
            inspection.HouseFeature = HouseInspectionDTO.HouseFeature;

            foreach (var img in HouseInspectionDTO.HouseImages)
            {
                var url = await r2StorageService.UploadFileAsync(img);
                inspection.HouseImages.Add(url);
            }
            await apiContext.SaveChangesAsync();

            await notificationService.SendNotificationAsync("طلب تأكيد للمعاينة", $"قام {user.Firstname} {user.Lastname}\n من خدمة العملاء بادخال البيانات المطلوبة, قم بزيارة ملفك الشخصي لتأكيد بيانات المعاينة.", landlord);
            return Ok(new { message = $"House Inespection completed successfully" });
        }

        [HttpPut("HouseInspection/acknowledge/{houseInspectionId}")]
        public async Task<IActionResult> EditInspectionStatus(string houseInspectionId)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
                return Forbid();

            var houseInspection = await apiContext.HouseInspections
                .Include(hi => hi.House)
                .FirstOrDefaultAsync(hi => hi.HouseInspectionId == houseInspectionId);

            if (houseInspection == null)
                return NotFound(new { message = "House inspection not found" });

            if (houseInspection.inspectionStatus != InspectionStatus.Bending)
                return BadRequest(new { message = "House inspection is not in bending status" });

            var landlord = await apiContext.Landlords
                .FirstOrDefaultAsync(l => l.Username == houseInspection.House.LandlordUsername);

            if (landlord == null)
                return NotFound(new { message = "Landlord not found" });

            houseInspection.inspectionStatus = InspectionStatus.InProgress;
            houseInspection.InspectorId = user.Username;

            await notificationService.SendNotificationAsync("طلب المعاينة قد التقدم", $"قام {user.Firstname} {user.Lastname}\nمن خدمة العملاء بتولي عملية معاينة المنزل\nتستطيع الآن التواصل معه عبر الدردشة الخاصة بالموقع لتحديد ميعاد المعاينة", landlord);
            await apiContext.SaveChangesAsync();
            return Ok(new { message = $"House inspection status updated to \"{InspectionStatus.InProgress}\" successfully", InspectorId = user.Username });
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

            var complaint = await apiContext.Complaints.Include(r => r.Tenant)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound(new { message = "Complaint not found" });

            if (complaint.status != ComplainStatus.Bending)
                return BadRequest(new { message = "Complaint is not in bending status" });

            complaint.status = ComplainStatus.InProgress;
            complaint.VerifiedBy = user.Username;
            await apiContext.SaveChangesAsync();

            await notificationService.SendNotificationAsync("تم استلام الشكوى", $"قام {user.Firstname} {user.Lastname}\nمن خدمة العملاء باستلام الشكوى الخاصة بك, تستطيع الآن التواصل معه عبر الدردشة الخاصة بالموقع.", complaint.Tenant);
            return Ok(new { message = $"Complaint status updated to {complaint.status} successfully", InspectorId = user.Username });
        }

        [HttpPut("Complaint/Review/{complaintId}")]
        public async Task<IActionResult> ReviewComplaint(string complaintId, [FromBody] ComplaintReviewDTO complaintReviewDTO)
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
            await apiContext.SaveChangesAsync();

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
                .Include(h => h.House).Where(e => e.inspectionStatus == InspectionStatus.Bending)
                .Select(h => new
                {
                    HouseInspectionId = h.HouseInspectionId,
                    HouseId = h.HouseId,
                    ownerId = h.House.LandlordUsername,
                    ownerFirstName = h.House.owner.Firstname,
                    ownerLastName = h.House.owner.Lastname,
                    SubmitedAt = h.InspectionRequestDate,
                    Governorate = h.House.governorate,
                    city = h.House.city,
                    street = h.House.street

                })
                .ToListAsync();

            return Ok(houseInspections);
        }

        [HttpPut("Return/WarrantyMoney/Tenant/{complaintId}")]
        public async Task<IActionResult> ReturnWarrantyMoneyToTenant(string complaintId)
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

            var complaint = await apiContext.Complaints
                .Include(c => c.Rental)
                .ThenInclude(r => r.Tenant)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound(new { message = "Complaint not found" });

            if (complaint.status != ComplainStatus.InProgress)
                return BadRequest(new { message = "Complaint is not in progress status" });

            var tenant = complaint.Rental.Tenant;
            if (tenant == null)
                return NotFound(new { message = "Tenant not found" });

            tenant.Balance += (int)(complaint.Rental.WarrantyMoney - complaint.Rental.WarrantyMoney * Rental.OurPercentage);
            complaint.status = ComplainStatus.NoBlock;
            await apiContext.SaveChangesAsync();

            await notificationService.SendNotificationAsync("تم استرداد مبلغ الضمان", $"تم استرداد مبلغ الضمان الخاص بك بنجاح, يمكنك الآن استخدامه في عمليات أخرى.", tenant);
            return Ok(new { message = "Warranty money returned successfully" });
        }

        [HttpPut("Return/WarrantyMoney/Lanlord/{complaintId}")]
        public async Task<IActionResult> ReturnWarrantyMoneyToLanlord(string complaintId)
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

            var complaint = await apiContext.Complaints
                .Include(r => r.Tenant)
                .Include(r => r.Rental)
                .ThenInclude(r => r.House)
                .ThenInclude(h => h.owner)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
                return NotFound(new { message = "Complaint not found" });

            if (complaint.status != ComplainStatus.InProgress)
                return BadRequest(new { message = "Complaint is not in progress status" });

            var landlord = complaint.Rental.House.owner;

            if (landlord == null)
                return NotFound(new { message = "Landlord not found" });

            landlord.Balance += (int)(complaint.Rental.WarrantyMoney * Rental.OurPercentage);
            complaint.status = ComplainStatus.NoBlock;
            await apiContext.SaveChangesAsync();
            await notificationService.SendNotificationAsync("تم استلام مبلغ ضمان", $"تم تحويل مبلغ الضمان الخاص بك بنجاح, لمعرفة رصيدك الحالي قم بزيارة ملفك الشخصي علي الموقع.", landlord);
            return Ok(new { message = "Warranty money transformed to landlord successfully" });
        }

        [HttpPost("BlockLandlord/{complaintId}")]
        public async Task<IActionResult> BlockLandlord(string complaintId, [FromBody] BlockIeddDTO blockIeddDTO)
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
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var complaint = await apiContext.FindAsync<Complaint>(id);
            if (complaint == null)
                return NotFound(new { message = "Complaint not found" });

            var landlord = await apiContext.Rentals
                 .Where(r => r.RentalId == complaint.RentalId)
                 .Select(r => r.House.owner)
                 .FirstOrDefaultAsync();

            if (landlord == null)
                return NotFound(new { message = "Landlord not found" });

            var isBlocked = await apiContext.BlockedIds.FindAsync(landlord.SocialId);
            if (isBlocked != null)
                return BadRequest(new { message = "Landlord is already blocked." });

            if (landlord.IsVerified == false)
                return BadRequest(new { message = "Landlord is not verified, cannot be blocked." });

            landlord.IsBlocked = true;
            var blockedId = new BlockedId
            {
                SocialId = landlord.SocialId,
                BlockedBy = user.Username,
                BlockedDate = DateTime.UtcNow,
                Reason = blockIeddDTO.Reason
            };
            var advs = await apiContext.Advertisements
                .Include(a => a.House)
                .Where(a => a.House.owner.Username == landlord.Username)
                .ToListAsync();

            apiContext.Advertisements.RemoveRange(advs);

            await apiContext.BlockedIds.AddAsync(blockedId);
            await apiContext.SaveChangesAsync();
            return Ok(new { message = "Landlord blocked successfully" });
        }
        [Authorize]
        [HttpGet("CustomerService/Inspictions")]
        public async Task<IActionResult> GetCustomerServiceInspictions()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null || user.UserType != UserType.Customerservice)
            {
                return Forbid();
            }

            var inspections = await apiContext.HouseInspections
                .Include(h => h.House)
                .Where(e => e.inspectionStatus == InspectionStatus.InProgress && e.InspectorId == username)
                .Select(h => new
                {
                    HouseInspectionId = h.HouseInspectionId,
                    HouseId = h.HouseId,
                    ownerId = h.House.LandlordUsername,
                    ownerFirstName = h.House.owner.Firstname,
                    ownerLastName = h.House.owner.Lastname,
                    SubmitedAt = h.InspectionRequestDate,
                    Governorate = h.House.governorate,
                    city = h.House.city,
                    street = h.House.street

                })
                .ToListAsync();

            return Ok(inspections);

        }
    }
}
