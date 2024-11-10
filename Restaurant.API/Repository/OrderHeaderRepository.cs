using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<OrderHeader> UpdateAsync(OrderHeader entity)
        {
            _db.OrderHeaders.Update(entity);
            return entity;
        }
    }
}
