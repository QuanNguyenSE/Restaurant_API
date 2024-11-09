using Microsoft.EntityFrameworkCore;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;
using System.Linq.Expressions;

namespace Restaurant.API.Repository
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        private readonly ApplicationDbContext _db;
        public CartItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<CartItem> UpdateAsync(CartItem entity)
        {
            _db.CartItems.Update(entity);
            return entity;
        }
    }
}
