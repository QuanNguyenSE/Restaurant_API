using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Restaurant.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private APIResponse _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string secretKey;
        public AuthController(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _response = new APIResponse();
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDTO resquestDTO)
        {
            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == resquestDTO.UserName);
            bool isValid = await _userManager.CheckPasswordAsync(user, resquestDTO.Password);
            if (!isValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Username or password is incorrect");
                _response.Result = new LoginResponseDTO();
                return _response;
            }

            var roles = await _userManager.GetRolesAsync(user);

            //generate JWT token
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secretKey);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("name", user.Name.ToString()),
                    new Claim("id", user.Id),
                    new Claim(ClaimTypes.Email, user.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Email = user.Email,
                Token = tokenHandler.WriteToken(token),
            };

            _response.Result = loginResponseDTO;
            _response.StatusCode = HttpStatusCode.OK;
            return _response;
        }

        [HttpPost("register")]
        public async Task<ActionResult<APIResponse>> Register([FromBody] RegisterRequestDTO resquestDTO)
        {
            ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == resquestDTO.UserName.ToLower());
            if (user != null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Username is exists");
                return _response;
            }
            // don't use mapper
            ApplicationUser newUser = new ApplicationUser()
            {
                UserName = resquestDTO.UserName,
                Email = resquestDTO.UserName,
                Name = resquestDTO.Name,
            };
            try
            {
                var result = await _userManager.CreateAsync(newUser, resquestDTO.Password);

                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                    {
                        //create roles in database
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_OnlCustomer));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Table));
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Staff));
                    }
                    if (resquestDTO.Role.ToLower() == SD.Role_Admin)
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
                    }
                    else if (resquestDTO.Role.ToLower() == SD.Role_Staff)
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Staff);
                    }
                    else if (resquestDTO.Role.ToLower() == SD.Role_Table)
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_Table);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(newUser, SD.Role_OnlCustomer);
                    }

                    _response.StatusCode = HttpStatusCode.OK;
                    _response.IsSuccess = true;

                }
            }
            catch (Exception)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while registering");
            }

            return _response;

        }


    }
}
