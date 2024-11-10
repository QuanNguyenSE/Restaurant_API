using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<OrderDetail> UpdateAsync(OrderDetail obj);
    }
}
