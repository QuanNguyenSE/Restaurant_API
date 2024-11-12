using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> UpdateAsync(ApplicationUser obj);
    }
}
