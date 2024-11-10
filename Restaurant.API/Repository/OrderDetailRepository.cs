using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<OrderDetail> UpdateAsync(OrderDetail entity)
        {
            _db.OrderDetails.Update(entity);
            return entity;
        }
    }
}
