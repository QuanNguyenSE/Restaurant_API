namespace Restaurant.API.Utility
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        CheckedIn, // khách đến
        Occupied, // đang sử dụng
        Completed, // khách rời đi
        Cancelled
    }
}
