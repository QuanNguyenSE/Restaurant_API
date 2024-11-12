namespace Restaurant.API.Models.DTO
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfGuests { get; set; }
        public string? SpecialRequest { get; set; }
        public DateTime DateCreated { get; set; }
        public string BookingStatus { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
