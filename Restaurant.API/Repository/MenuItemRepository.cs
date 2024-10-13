using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;
        public MenuItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<MenuItem> UpdateAsync(MenuItem entity)
        {
            _db.MenuItems.Update(entity);
            return entity;
        }
    }
}
