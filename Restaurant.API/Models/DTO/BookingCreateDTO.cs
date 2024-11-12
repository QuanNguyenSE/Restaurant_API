using Restaurant.API.Utility;

namespace Restaurant.API.Models.DTO
{
    public class BookingCreateDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? SpecialRequest { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
