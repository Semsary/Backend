using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using semsary_backend.EntityConfigurations;
using semsary_backend.Service;

namespace semsary_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController(ApiContext apiContext) : ControllerBase
    {
        [HttpGet("User/Info/{username}")]
        public async Task<IActionResult> UserInfo(string username)
        {
            var user = await apiContext.SermsaryUsers
                .FirstOrDefaultAsync(e => e.Username == username);
            if (user == null)
                return NotFound("User not found");

            var userData = new
            {
                firstName = user.Firstname,
                lastName = user.Lastname,
                profilePic = user.ProfileImageUrl
            };
            return Ok(userData);

        }
    }
}
