using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<Booking> UpdateAsync(Booking obj);
    }
}
