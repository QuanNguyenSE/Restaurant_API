using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<ShoppingCart> UpdateAsync(ShoppingCart entity)
        {
            _db.ShoppingCarts.Update(entity);
            return entity;
        }
    }
}
