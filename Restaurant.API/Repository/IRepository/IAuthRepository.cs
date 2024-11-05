using Restaurant.API.Models.DTO;

namespace Restaurant.API.Repository.IRepository
{
    public interface IAuthRepository
    {
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO requestDTO);
        Task<UserDTO> RegisterAsync(RegisterRequestDTO requestDTO);
    }
}
