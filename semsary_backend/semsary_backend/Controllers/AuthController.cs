using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Generators;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Models;
using semsary_backend.Service;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private EmailService _emailService;
        private readonly TokenService tokenGenertor;
        private readonly ApiContext apiContext;
        private readonly R2StorageService storageService;
        private readonly NotificationService notificationService;

        public AuthController(EmailService emailService, TokenService TokenGenertor, ApiContext apiContext,R2StorageService storageService,NotificationService notificationService)
        {
            _emailService = emailService;
            tokenGenertor = TokenGenertor;
            this.apiContext = apiContext;
            this.storageService = storageService;
            this.notificationService = notificationService;
        }


        [HttpPost("Landlord/register")]
        public async Task<IActionResult> RegisterLandlord([FromBody] SignUpLandlord landlordDTO)
        {
            if (landlordDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var user = apiContext.SermsaryUsers.Include(w => w.Emails).FirstOrDefault(e => (e.Emails.Any(Em => Em.email == landlordDTO.Email)));
            if (user != null)
            {
                return BadRequest("Email already exists.");
            }
            var UserEmail = new Email
            {
                email = landlordDTO.Email,
                IsVerified = false,
                otp = GenerateOtp.Generate(6),
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                NumberofMismatch = 0,
            };

            var landlord = new Landlord
            {
                Firstname = landlordDTO.firstname,
                Lastname = landlordDTO.lastname,
                password = PasswordHelper.HashPassword(landlordDTO.Password),
                UserType = UserType.landlord

            };
            UserEmail.owner = landlord;
            UserEmail.ownerUsername = landlord.Username;
            landlord.Emails = new List<Email>();
            landlord.Emails.Add(UserEmail);
            var otp = UserEmail.otp;
            var email = UserEmail.email;
            await _emailService.SendEmailAsync(email, "verification request", "to verify your email please enter this otp on the website " + otp + "if you don't ask this otp please ignore this message");
            apiContext.SermsaryUsers.Add(landlord);
            await apiContext.SaveChangesAsync();
            return Ok("Registration successful. Please check your email for verification.");

        }


        [HttpPost("Tenant/register")]
        public async Task<IActionResult> RegisterTenant([FromBody] SignUpLandlord TenantDTO)
        {
            if (TenantDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var user = apiContext.SermsaryUsers.Include(w => w.Emails).FirstOrDefault(e => (e.Emails.Any(Em => Em.email == TenantDTO.Email)));
            if (user != null)
            {
                return BadRequest("Email already exists.");
            }
            var UserEmail = new Email
            {
                email = TenantDTO.Email,
                IsVerified = false,
                otp = GenerateOtp.Generate(6),
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                NumberofMismatch = 0,

            };

            var Tenant = new Tenant
            {
                Firstname = TenantDTO.firstname,
                Lastname = TenantDTO.lastname,
                password = PasswordHelper.HashPassword(TenantDTO.Password),
                UserType = UserType.Tenant


            };
            UserEmail.owner = Tenant;
            UserEmail.ownerUsername = Tenant.Username;
            Tenant.Emails = new List<Email>();
            Tenant.Emails.Add(UserEmail);
            var otp = UserEmail.otp;
            var email = UserEmail.email;
            await _emailService.SendEmailAsync(email, "verification request", "to verify your email please enter this otp on the website " + otp + "\nif you don't ask this otp please ignore this message");
            apiContext.SermsaryUsers.Add(Tenant);
            await apiContext.SaveChangesAsync();
            return Ok("Registration successful. Please check your email for verification.");

        }



        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
        {
            if (verifyEmailDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var user = await apiContext.SermsaryUsers.Include(e => e.Emails)
                .FirstOrDefaultAsync(u => u.Emails.Any(e => e.email == verifyEmailDTO.Email));

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var email = user.Emails.FirstOrDefault(e => e.email == verifyEmailDTO.Email);

            if (email == null || email.otp != verifyEmailDTO.Otp || email.OtpExpiry < DateTime.UtcNow)
            {
                return BadRequest("Invalid OTP or OTP has expired.");
            }

            email.IsVerified = true;
            await apiContext.SaveChangesAsync();

            return Ok("Email verified successfully.");
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (loginDTO == null)
                return BadRequest();
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);
            var email = await apiContext.Emails.Include(em => em.owner).FirstOrDefaultAsync(e => e.email == loginDTO.Email);
            if (email == null)
                return NotFound("this email wasn't found");
            if (email.IsVerified == false)
                return BadRequest("This Email isn't verified,please verify it or ask to delete it");

            var user = email.owner;
            if (PasswordHelper.VerifyPassword(loginDTO.Password, user.password) == false)
                return BadRequest("invalid email or password");

            var token = tokenGenertor.GenerateToken(user);

            bool askForNotificationPermission = false;
            if (user.UserType == UserType.Tenant)
            {
                Tenant tenant = (Tenant)user;
                askForNotificationPermission =  tenant.DeviceTokens == null
                                                || !tenant.DeviceTokens.Contains(loginDTO.deviceToken);
            }
            else if(user.UserType == UserType.landlord)
            {
                Landlord landlord = (Landlord)user;
                askForNotificationPermission =  landlord.DeviceTokens == null 
                                                || !landlord.DeviceTokens.Contains(loginDTO.deviceToken);
            }
            else if(user.UserType == UserType.Customerservice)
            {
                CustomerService customer = (CustomerService)user;
                askForNotificationPermission = customer.DeviceTokens == null
                                                || !customer.DeviceTokens.Contains(loginDTO.deviceToken);
            }
           
            var response = new 
            {
                Token = token,
                AskForNotificationPermission = askForNotificationPermission
            };
            return Ok(response);
        }

        [Authorize] 
        [HttpPost("AllowNotifications")]
        public async Task<IActionResult> SaveDeviceToken([FromBody] DeviceTokenDTO deviceToken)
        {
            if (string.IsNullOrWhiteSpace(deviceToken.DeviceToken))
                return BadRequest("Device token is required.");

            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.UserType == UserType.Tenant)
            {
                Tenant tenant = (Tenant)user;
                if (tenant.DeviceTokens == null)
                    tenant.DeviceTokens = new List<string>();
                if (!tenant.DeviceTokens.Contains(deviceToken.DeviceToken))
                {
                    tenant.DeviceTokens.Add(deviceToken.DeviceToken);
                    await apiContext.SaveChangesAsync();
                }
            }
            else if (user.UserType == UserType.landlord)
            {
                Landlord landlord = (Landlord)user;
                if (landlord.DeviceTokens == null)
                    landlord.DeviceTokens = new List<string>();

                if (!landlord.DeviceTokens.Contains(deviceToken.DeviceToken))
                {
                    landlord.DeviceTokens.Add(deviceToken.DeviceToken);
                    await apiContext.SaveChangesAsync();
                }
            }
            else if(user.UserType == UserType.Customerservice)
            {
                CustomerService customer = (CustomerService)user;
                if (customer.DeviceTokens == null)
                    customer.DeviceTokens = new List<string>();
                if (!customer.DeviceTokens.Contains(deviceToken.DeviceToken))
                {
                   customer.DeviceTokens.Add(deviceToken.DeviceToken);
                    await apiContext.SaveChangesAsync();
                }
            }
            else
            {
                return Forbid("This user type doesn't support notifications.");
            }
            return Ok("Sending notifications is allowed successfully.");
        }
        

        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetCurUser()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .Include(e => e.Emails)
                .FirstOrDefaultAsync(e => e.Username == username);
            if (user == null)
                return NotFound("User not found.");
            var userInfo = new {
                firstname= user.Firstname,
                lastname = user.Lastname,
                username = user.Username,
                emails = user.Emails.Select(e => e.email).ToList(),
                address = user.Address,
                userType = user.UserType

            };

            return Ok(userInfo);
        }

        [HttpGet("UpdatePasword")]
        public async Task<IActionResult> UpdatePassword(string email)
        {
            var Em = await apiContext.Emails.FirstOrDefaultAsync(e => e.email == email);
            if (Em == null)
            {
                return BadRequest("Email not found");
            }
            if (Em.IsVerified == false)
                return BadRequest("This Email isn't verified,please verify it");
            Em.otpType = OTPType.PasswordReset;
            Em.otp = GenerateOtp.Generate();
            Em.NumberofMismatch = 0;
            Em.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            String subject = "Reset password";
            String body = "your reset password otp is " + Em.otp + " \n if you didn't ask to reset your password,please ignore this message";
            await _emailService.SendEmailAsync(email, subject, body);
            await apiContext.SaveChangesAsync();
            return Ok("an otp was sent to your email");


        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> resetpassword(ResetPasswordDTO resetdto)
        {
            if (resetdto == null)
                return BadRequest("invalid input");
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);
            var email = await apiContext.Emails.Include(e => e.owner).FirstOrDefaultAsync(e => e.email == resetdto.Email);
            if (email == null)
                return NotFound("this email wasn't found");

            if (email.IsVerified == false)
                return BadRequest("This Email isn't verified,please verify this email first");
            if (email.OtpExpiry < DateTime.UtcNow)
                return BadRequest("expired otp");

            if (email.NumberofMismatch > 3)
                return BadRequest("you have reached maximum number of attempts");


            if (email.otp != resetdto.Otp)
            {
                email.NumberofMismatch++;
                apiContext.SaveChanges();
                return BadRequest("wrong otp");
            }
            email.NumberofMismatch = 0;
            email.OtpExpiry = DateTime.MinValue;
            email.owner.password = PasswordHelper.HashPassword(resetdto.Password);
            await apiContext.SaveChangesAsync();

            return Ok("password updated successfully");

        }
        [Authorize]
        [HttpPost("Customerservice/Add")]
        public async Task<IActionResult> Add(CustomerserviceDTO dto)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers.FirstOrDefaultAsync(e => e.Username == username);
            if (user.UserType != UserType.Admin)
                return Unauthorized("you are not authorized to add customer service");

            if (dto == null)
                return BadRequest("invalid input");
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var em = apiContext.Emails.FirstOrDefault(e => e.email == dto.email);
            if (em != null)
            {
                return BadRequest("this email is already taken");
            }
            var email = new Email
            {
                email = dto.email,
                IsVerified = false,
                otp = GenerateOtp.Generate(6),
                OtpExpiry = DateTime.UtcNow.AddMinutes(5),
                NumberofMismatch = 0,
            };
            var customerService = new CustomerService
            {
                Firstname = dto.firstname,
                Lastname = dto.lastname,
                password = PasswordHelper.HashPassword(dto.password),
                UserType = UserType.Customerservice,
                Emails = new List<Email> { email }
            };
            email.owner = customerService;
            email.ownerUsername = customerService.Username;
            await _emailService.SendEmailAsync(email.email, "verification request", "to verify your email please enter this otp on the website " + email.otp + "\nif you don't ask this otp please ignore this message");
            apiContext.SermsaryUsers.Add(customerService);
            await apiContext.SaveChangesAsync();
            // Send verification email


            return Ok("customer service was created successfully");

        }
        [HttpGet("GetVerifictioncode")]
        public async Task<IActionResult> GetVerifictioncode(string email)
        {
            var em = await apiContext.Emails.FirstOrDefaultAsync(e => e.email == email);
            if (em == null)
                return NotFound("this email wasn't found");
            if (em.IsVerified == true)
                return BadRequest("this email is already verified");

            em.otp = GenerateOtp.Generate(6);
            em.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            em.NumberofMismatch = 0;
            em.otpType = OTPType.Verification;

            await _emailService.SendEmailAsync(email, "verification request", "to verify your email please enter this otp on the website " + em.otp + "\nif you don't ask this otp please ignore this message");
            await apiContext.SaveChangesAsync();
            return Ok("verification code was sent to your email");
        }
        [HttpGet("GetDeletionCode")]
        public async Task<IActionResult> GetDeletionCode(string email)
        {
            var em = await apiContext.Emails.FirstOrDefaultAsync(e => e.email == email);
            if (em == null)
                return NotFound("this email wasn't found");
            if (em.IsVerified)
                return BadRequest("this email is already verified");

            em.otp = GenerateOtp.Generate(6);
            em.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
            em.NumberofMismatch = 0;
            em.otpType = OTPType.Deletion;

            await _emailService.SendEmailAsync(email, "Delettion request", "to delete your email please enter this otp on the website " + em.otp + "\nif you don't ask this otp please ignore this message");
            await apiContext.SaveChangesAsync();
            return Ok("deletion code was sent to your email");

        }
        [HttpPost("DeleteEmail")]
        public async Task<IActionResult> DeleteEmail(VerifyEmailDTO verifyEmailDTO)
        {
            if (verifyEmailDTO == null)
                return BadRequest("invalid input");
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var email = await apiContext.Emails.Include(e => e.owner).FirstOrDefaultAsync(e => e.email == verifyEmailDTO.Email);
            if (email == null)
                return NotFound("this email wasn't found");

            if (email.IsVerified)
                return BadRequest("this email is already verified");
            if (email.otpType != OTPType.Deletion)
                return BadRequest("this otp isn't for deletion");

            if (email.OtpExpiry < DateTime.UtcNow)
                return BadRequest("expired otp");

            if (email.NumberofMismatch > 3)
                return BadRequest("you have reached maximum number of attempts");
            if (email.otp != verifyEmailDTO.Otp)
            {
                email.NumberofMismatch++;
                apiContext.SaveChanges();
                return BadRequest("wrong otp");
            }
            var owner = email.owner;
            apiContext.Emails.Remove(email);
            apiContext.SermsaryUsers.Remove(owner);
            return Ok("email was deleted successfully");


        }
        
        [Authorize]
        [HttpPost("SubmitIdentity")]
        public async Task<IActionResult> SubmitIdentity(IdentityDTO iddto)
        {
            if(iddto == null)
                return BadRequest("invalid input");
            if (ModelState.IsValid == false)    
                return BadRequest(ModelState);
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.UnverifiedUsers.Include(e => e.Emails).FirstOrDefaultAsync(e => e.Username == username);
            if (user == null)
                return NotFound("User not found.");
            if(user.IsVerified)
                return BadRequest("this user is already verified");
            List<string> UserImages =new List<string>
            {
                await storageService.UploadFileAsync(iddto.UserImage),
                await storageService.UploadFileAsync(iddto.IdentityFront),
               await  storageService.UploadFileAsync(iddto.IdentityBack)
            };
            var identity = new IdentityDocument
            {
                OwnerUsername = user.Username,
                ImageURLS = UserImages,
                Status = IdentityStatus.Pending,
                Id = Ulid.NewUlid().ToString(),
                Owner = user
            };
            user.Identity.Add(identity);
            apiContext.identityDocuments.Add(identity);
            await apiContext.SaveChangesAsync();
            return Ok("identity was submitted successfully");

        }
        
        [Authorize]
        [HttpGet("GetIdentity")]
        
        public async Task<IActionResult> GetIdentity()
        {
            var username= tokenGenertor.GetCurUser();
            var user = await apiContext.UnverifiedUsers.Include(e => e.Identity).FirstOrDefaultAsync(e => e.Username == username);
            if (user == null)
                return NotFound("User not found.");
            
            if (user.Identity.Count == 0)
                return NoContent();
            
            return Ok(user.Identity);

        }
        [Authorize]
        [HttpGet("GetAllIdentity")]
        public async Task<IActionResult> GetAllIdentity()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers.FirstOrDefaultAsync(e => e.Username == username);
            if (user == null)
                return NotFound("User not found.");
            if (user.UserType != UserType.Customerservice)
                return Unauthorized("you are not authorized to get all identity");

            var identities = await apiContext.identityDocuments.Where(e=>e.Status==IdentityStatus.Pending)
                .Select(d=> new
                {
                    ownerUsername= d.OwnerUsername,
                    Id = d.Id,
                    ownerFirstName = d.Owner.Firstname,
                    ownerLastName = d.Owner.Lastname,

                    imageURLS = d.ImageURLS,
                    SubmitedAt=d.SubmitedAt,
                })
                .ToListAsync();
            if (identities.Count == 0)
                return NoContent();

            return Ok(identities);

        }
        [Authorize]
        [HttpPost("ReviewIdentity")]
        public async Task<IActionResult> ReviewIdentity(string id, IdentityStatus status, string comment)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers.FirstOrDefaultAsync(e => e.Username == username);
            if (user == null)
                return NotFound("User not found.");
            if (user.UserType != UserType.Customerservice)
                return Unauthorized("you are not authorized to review identity");
            var identity = await apiContext.identityDocuments.Include(e => e.Owner).FirstOrDefaultAsync(e => e.Id == id);
            if (identity == null)
                return NotFound("this identity wasn't found");
            if (status==IdentityStatus.Pending)
                return BadRequest("you can't set the status to pending,only Verified or Rejected are allowed");

            identity.Status = status;
            identity.Comment = comment;
            identity.ReviewerUsername = user.Username;
            identity.reviewer = (CustomerService)user;
            identity.ReviewedAt = DateTime.UtcNow;
            if (status == IdentityStatus.Verified)
                identity.Owner.IsVerified = true;
            else if (status == IdentityStatus.Rejected)
            {
                identity.Owner.IsVerified = false;

            }
           
            
            var message = status == IdentityStatus.Verified 
                ? "Your identity has been verified successfully." 
                : "Your identity verification has been rejected. Comment: " + comment;
            var title= status == IdentityStatus.Verified 
                ? "Identity Verified" 
                : "Identity Rejected";


            // Send notification to the user not email 
            if (identity.Owner.UserType == UserType.Tenant)
            {
                Tenant? tenant = apiContext.Tenant.Where(r => r.Username == identity.Owner.Username).FirstOrDefault();
                if (tenant.DeviceTokens != null && tenant.DeviceTokens.Count > 0)
                {
                    var notification = new Notification
                    {
                        Title = title,
                        Message = message,
                        SentTo = tenant.Username,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await apiContext.Notifications.AddAsync(notification);
                    await notificationService.SendNotificationAsync(title, message,tenant);
                }
            }
            else if (identity.Owner.UserType == UserType.landlord)
            {
                Landlord? landlord = apiContext.Landlords.Where(r => r.Username == identity.Owner.Username).FirstOrDefault();
                if (landlord.DeviceTokens != null && landlord.DeviceTokens.Count > 0)
                {
                    var notifcation = new Notification
                    {
                        Title = title,
                        Message = message,
                        SentTo = landlord.Username,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await apiContext.Notifications.AddAsync(notifcation);
                    await notificationService.SendNotificationAsync(title,message ,landlord);
                }
            }

            await apiContext.SaveChangesAsync();
            return Ok("identity was reviewed successfully");
        }

        [Authorize]
        [HttpPut("Edit/Profile")]
        public async Task<IActionResult> EditProfile([FromForm] EditProfileDTO edit)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .Include(e => e.Emails)
                
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return NotFound("User not found.");

            if (edit == null)
                return BadRequest("Invalid input data.");

            if (edit.Firstname != null)
                user.Firstname = edit.Firstname;

            if (edit.Lastname != null)
                user.Lastname = edit.Lastname;


            if(edit.gover != null)
            {
                user.Address = new Address();
                user.Address._gover = edit.gover.Value;
                if (edit.city != null)
                    user.Address._city = edit.city;
                if (edit.street != null)
                    user.Address.street = edit.street;
            }
            else
            {
                user.Address = new Address();
                user.Address._gover = Governorate.Cairo; // Default value if not provided
            }
            user.ProfileImageUrl =edit.ProfileImage is null?user.ProfileImageUrl: await storageService.UploadFileAsync( edit.ProfileImage);

            if (user.UserType == UserType.Tenant)
            {
                Tenant tenant = (Tenant)user;
                tenant.height = edit.height == null ? 160 : edit.height.Value;
                tenant.weight = edit.weight == null ? 70 : (int)edit.weight.Value;
                tenant.age = edit.age == null ? 20 : edit.age.Value;
                tenant.NumberOfPeople = edit.NumberOfPeople == null ? 1 : edit.NumberOfPeople.Value;
                tenant.FavouriteRentalType = edit.FavouriteRentalType == null ? RentalType2.Monthly : edit.FavouriteRentalType.Value;
                tenant.gender = edit.gender == null ? 0 : edit.gender.Value;
                tenant.IsSmoker = edit.IsSmoker == null ? false : edit.IsSmoker.Value;
                tenant.NeedNearUniversity = edit.NeedNearUniversity == null ? false : edit.NeedNearUniversity.Value;
                tenant.NeedNearVitalPlaces = edit.NeedNearVitalPlaces == null ? false : edit.NeedNearVitalPlaces.Value;
                tenant.NeedPublicService = edit.NeedPublicService == null ? false : edit.NeedPublicService.Value;
                tenant.NeedPublicTransportation = edit.NeedPublicTransportation == null ? false : edit.NeedPublicTransportation.Value;
                tenant.CompletedProfile = true;
            }

            apiContext.SaveChanges();

            return Ok("Profile updated successfully.");
        }


        [Authorize]
        [HttpGet("Auth/Me/")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .Include(e => e.Emails)
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return NotFound("User not found.");

            object otherTenantData = null;
            object otherLanlordData = null;
            var basicUserInfo = new
            {
                user.Username,
                user.Firstname,
                user.Lastname,
                user.ProfileImageUrl,
                Emails = user.Emails.Select(e => e.email).ToList(),
                Address = user.Address,
                userType = user.UserType
            };
            if (user.UserType == UserType.Tenant)
            {
                Tenant tenant = (Tenant)user;
                otherTenantData = new
                {
                    isSmoker = tenant.IsSmoker,
                    gender = tenant.gender,
                    age = tenant.age,
                    tenant.height,
                    tenant.Balance,
                    needPublicService = tenant.NeedPublicService,
                    needPublicTransportation = tenant.NeedPublicTransportation,
                    needNearUniversity = tenant.NeedNearUniversity,
                    needNearVitalPlaces = tenant.NeedNearVitalPlaces,
                    isVerified = tenant.IsVerified,
                    IsPremium = tenant.PremiumEnd >= DateTime.UtcNow,
                };
            }
            else if (user.UserType == UserType.landlord)
            {
                Landlord landlord = (Landlord)user;
                otherLanlordData = new
                {
                    landlord.Balance,
                    landlord.IsVerified,
                    landlord.IsBlocked
                };
            }
            return Ok(new { basicUserInfo, otherTenantData, otherLanlordData });
        }
        [Authorize]
        [HttpGet("Balance/Info")]
        public async Task<IActionResult> PreimumInfo()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return NotFound("User not found.");

            if (user.UserType == UserType.Tenant && user is Tenant tenant)
            {
                var info = new
                {
                    UserType = UserType.Tenant,
                    Isverified = tenant.IsVerified,
                    Balance = tenant.Balance,
                    IsPremium = tenant.PremiumEnd >= DateTime.UtcNow
                };
                return Ok(info);
            }

            if (user.UserType == UserType.landlord && user is Landlord landlord)
            {
                var info = new
                {
                    UserType = UserType.landlord,
                    Isverified = landlord.IsVerified,
                    Balance = landlord.Balance
                };
                return Ok(info);
            }
            return BadRequest("Only tenants and landlords can access this information.");
        }
        [HttpGet("User/Notifications")]
        public async Task<IActionResult> GetNotifications()
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);

            if (user == null)
                return NotFound("User not found");

            var notifications = await apiContext.Notifications
                .Where(n => n.SentTo == user.Username)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notifications);
        }
        [Authorize]
        [HttpPut("Deposit/{Id}")]
        public async Task<IActionResult> Deposit(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("invalid Id");

            var UserId = tokenGenertor.GetCurUser();
            if (string.IsNullOrEmpty(UserId))
                return Unauthorized("invalid username");
            var user = await apiContext.SermsaryUsers.Where(c => c.Username == UserId).FirstOrDefaultAsync();
            if (user == null)
                return BadRequest("this user doesn't exist");
            var Coupon = apiContext.Coupons.Where(c => c.Id.ToString() == Id).FirstOrDefault();
            if (Coupon == null)
                return BadRequest("this Coupon doesn't exist");
            if (Coupon.IsUsed)
                return BadRequest("this coupon is used before");
            if (user.UserType == UserType.Customerservice)
            {
                Coupon.ExPosedPy = UserId;
                Coupon.IsUsed = true;
                apiContext.SaveChanges();
            }
            else if (user.UserType == UserType.Admin)
                return BadRequest("this endpoint isn't designed to be used by the admin");
            Coupon.ExPosedPy = UserId;
            Coupon.IsUsed = true;
            var uder = (UnverifiedUser)user;
            uder.Balance += Coupon.Balance;
            apiContext.SaveChanges();

            return Ok("The selected coupon has been used successfully");


        }
        [Authorize]
        [HttpPut("withdraw")]
        public async Task<IActionResult> withdraw(int Balance)
        {
            var Id = tokenGenertor.GetCurUser();
            if (string.IsNullOrEmpty(Id))
                return BadRequest("invalid token");
            var u = apiContext.SermsaryUsers.Where(c => c.Username == Id).FirstOrDefault();
            if (u == null)
                return BadRequest("this user doesn't exist");
            if (u.UserType == UserType.Admin)
                return BadRequest("this endpoint isn't designed to be used by the admin");
            if (Balance < 1 || Balance >= 50000)
                return BadRequest("you have exeeded the allowed coupon value");

            if (u.UserType == UserType.Customerservice)
            {
                var coupon = new Coupon();
                coupon.Balance = Balance;
                coupon.CreatedBy = Id;
                coupon.CreatedBy = Id;
                apiContext.Coupons.Add(coupon);

                await apiContext.SaveChangesAsync();
                return Ok(new
                {
                    CouponId = coupon.Id.ToString()
                });

            }
            var RealUder = (UnverifiedUser)u;
            if (RealUder.IsVerified == false)
                return BadRequest("you are not verified yet");
            if (RealUder.IsBlocked)
                return BadRequest("you has been blocked");
            if (RealUder.Balance < Balance)
                return BadRequest("you don't have enough balance");
            RealUder.Balance -= Balance;
            var Coupon = new Coupon();
            Coupon.Balance = Balance;
            Coupon.CreatedBy = Id;
            apiContext.Coupons.Add(Coupon);

            apiContext.SaveChanges();
            return Ok(new
            {
                CouponId = Coupon.Id.ToString()
            });


        }


    }
}
