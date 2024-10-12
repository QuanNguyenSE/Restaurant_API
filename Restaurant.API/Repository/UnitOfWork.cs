using Restaurant.API.Data;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IMenuItemRepository MenuItem { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            MenuItem = new MenuItemRepository(_db);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
