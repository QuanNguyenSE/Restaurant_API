using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        Task<MenuItem> UpdateAsync(MenuItem obj);
    }
}
