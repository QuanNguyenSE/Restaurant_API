using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<ApplicationUser> UpdateAsync(ApplicationUser entity)
        {
            _db.ApplicationUsers.Update(entity);
            return entity;
        }
    }
}
