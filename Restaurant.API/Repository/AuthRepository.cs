using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Restaurant.API.Models;
using Restaurant.API.Models.DTO;
using Restaurant.API.Repository.IRepository;
using Restaurant.API.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Restaurant.API.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO requestDTO)
        {
            var user = await _userManager.FindByNameAsync(requestDTO.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, requestDTO.Password))
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles.FirstOrDefault());
            UserDTO userDTO = _mapper.Map<UserDTO>(user);
            userDTO.Role = roles.FirstOrDefault();
            return new LoginResponseDTO
            {
                User = userDTO,
                Token = token
            };
        }

        public async Task<UserDTO> RegisterAsync(RegisterRequestDTO requestDTO)
        {

            ApplicationUser user = new ApplicationUser
            {
                UserName = requestDTO.UserName,
                Email = requestDTO.UserName,
                NormalizedEmail = requestDTO.UserName.ToUpper(),
                Name = requestDTO.Name,
                PhoneNumber = requestDTO.PhoneNumber,
                StreetAddress = requestDTO.StreetAddress,
                City = requestDTO.City,
                State = requestDTO.State,
            };

            var result = await _userManager.CreateAsync(user, requestDTO.Password);
            var userDto = _mapper.Map<UserDTO>(user);
            if (result.Succeeded)
            {
                bool isValid = await _roleManager.RoleExistsAsync(requestDTO.Role);
                if (isValid)
                {
                    await _userManager.AddToRoleAsync(user, requestDTO.Role ?? SD.Role_OnlCustomer);
                    userDto.Role = requestDTO.Role;
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_OnlCustomer);
                    userDto.Role = SD.Role_OnlCustomer;
                }
                return userDto;
            }
            else
                return null;

        }

        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["ApiSettings:Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
