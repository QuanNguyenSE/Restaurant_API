using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        Task<ShoppingCart> UpdateAsync(ShoppingCart obj);
    }
}
