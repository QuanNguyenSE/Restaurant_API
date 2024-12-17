using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using System.Net;

namespace Restaurant.API.Controllers
{
    //[Authorize(Roles = SD.Role_Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APIResponse _response;

        public UserController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _response = new APIResponse();
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                
                foreach (var user in users)
                {
                    user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                }
                _response.Result = users;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse>> GetUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    throw new Exception("Not found");
                }

                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

                _response.Result = user;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        [HttpPost]
        public async Task<ActionResult<APIResponse>> CreateUser([FromForm] RegisterRequestDTO requestDTO)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(requestDTO.UserName);

                if (user != null)
                {
                    throw new Exception("UserName exists");
                }

                var userDto = await _unitOfWork.Auth.RegisterAsync(requestDTO);

                if (userDto == null)
                {
                    throw new Exception("Error while registering");
                }
                _response.Result = userDto;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRoleToUser(string id, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null)
                {
                    throw new Exception("User does not exists");
                }

                bool isValid = await _roleManager.RoleExistsAsync(role);
                if (isValid)
                {
                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    throw new Exception("Role is not valid");
                }
                var userDto = new UserDTO
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    Role = role
                };
                _response.Result = userDto;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
    }

}
