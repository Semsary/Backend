using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using semsary_backend.DTO;
using semsary_backend.EntityConfigurations;
using semsary_backend.Enums;
using semsary_backend.Models;
using semsary_backend.Service;
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

        public AuthController(EmailService emailService,TokenService TokenGenertor,ApiContext apiContext)
        {
            _emailService = emailService;
            tokenGenertor = TokenGenertor;
            this.apiContext = apiContext;
        }

        
        [HttpPost("Landlord/register")]
        public async Task<IActionResult> RegisterLandlord([FromBody] SignUpLandlord landlordDTO)
        {
            if (landlordDTO == null)
            {
                return BadRequest("Invalid data.");
            }
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
            var user=  apiContext.SermsaryUsers.Include(w=>w.Emails).FirstOrDefault(e=>(e.Emails.Any(Em =>Em.email==landlordDTO.Email)));
            if (user != null)
            {
                return BadRequest("Email already exists.");
            }
            var UserEmail= new Email
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
                UserType=UserType.landlord


            };

            UserEmail.owner = landlord;
            UserEmail.ownerUsername= landlord.Username;  
            landlord.Emails = new List<Email>();
            landlord.Emails.Add(UserEmail);
            var otp= UserEmail.otp;
            var email = UserEmail.email;
            await _emailService.SendEmailAsync(email,"verification request","to verify your email please enter this otp on the website "+ otp +"if you don't ask this otp please ignore this message");
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
            var user = await apiContext.SermsaryUsers.Include(e=> e.Emails)
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


        [HttpGet("getLandlord")]
        public async Task<IActionResult> GetLandlord()
        {
            var landlords = await apiContext.SermsaryUsers
                .OfType<Landlord>()
                .ToListAsync();

            if (landlords == null || landlords.Count == 0)
            {
                return NotFound("No landlords found.");
            }

            return Ok(landlords);
        }
        
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if(loginDTO ==null)
                return BadRequest();
            if (ModelState.IsValid==false)
                return BadRequest(ModelState);
            
            var user= apiContext.SermsaryUsers.FirstOrDefault(e=>e.Emails.Any(m=>m.email==loginDTO.Email && m.IsVerified));
            if (user == null || PasswordHelper.VerifyPassword(loginDTO.Password,user.password)==false)
                return BadRequest("invalid email or password");
            return Ok(tokenGenertor.GenerateToken(user));

        }
        [Authorize]
        [HttpGet("GetCurUser")]
        public async Task<IActionResult> GetCurUser()
        {
            return Ok(tokenGenertor.GetCurUser());
        }

        [HttpGet("UpdatePasword")]
        public async Task<IActionResult> UpdatePassword(string email)
        {
             var Em=await  apiContext.Emails.FirstOrDefaultAsync(e => e.email == email);
            if(Em == null)
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
            String body = "your reset password otp is "+Em.otp+" \n if you didn't ask to reset your password,please ignore this message";
            await _emailService.SendEmailAsync(email,subject,body);
            await apiContext.SaveChangesAsync();
            return Ok("an otp was sent to your email");


        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> resetpassword(ResetPasswordDTO resetdto)
        {
            if (resetdto == null)
                return BadRequest("invalid input");
            if (ModelState.IsValid==false)
                return BadRequest(ModelState);
            var email =await apiContext.Emails.Include(e=>e.owner).FirstOrDefaultAsync(e=>e.email==resetdto.Email);
            if (email == null)
                return NotFound("this email wasn't found");

            if (email.IsVerified == false) 
                return BadRequest("This Email isn't verified,please verify this email first");
            if (email.OtpExpiry < DateTime.UtcNow)
                return BadRequest("expired otp");

            if (email.NumberofMismatch > 3)
                return BadRequest("you have reached maximum number of attempts");

            
            if(email.otp!=resetdto.Otp)
            {
                email.NumberofMismatch++;
                apiContext.SaveChanges();
                return BadRequest("wrong otp");
            }
            email.NumberofMismatch = 0;
            email.OtpExpiry = DateTime.MinValue;
            email.owner.password=PasswordHelper.HashPassword(resetdto.Password);
            await apiContext.SaveChangesAsync();

            return Ok("password updated successfully");

        }
        [Authorize]
        [HttpPost("Customerservice/Add")]
        public async Task<IActionResult> Add(CustomerserviceDTO dto)
        {
            var username = tokenGenertor.GetCurUser();
            var user = await apiContext.SermsaryUsers.FirstOrDefaultAsync(e => e.Username == username);
            if(user.UserType!=UserType.Admin)
                return Unauthorized("you are not authorized to add customer service");

            if (dto == null)
                return BadRequest("invalid input");
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var em=apiContext.Emails.FirstOrDefault(e => e.email == dto.email);
            if(em!=null)
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
       
    }
}
