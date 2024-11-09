using Restaurant.API.Models;
using System.Linq.Expressions;

namespace Restaurant.API.Repository.IRepository
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<CartItem> UpdateAsync(CartItem obj);
    }
}
