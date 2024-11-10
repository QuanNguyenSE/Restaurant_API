using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        Task<OrderHeader> UpdateAsync(OrderHeader obj);
    }
}
