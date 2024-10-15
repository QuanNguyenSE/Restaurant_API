using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> UpdateAsync(Category obj);
    }
}
