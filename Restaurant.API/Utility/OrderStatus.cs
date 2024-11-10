namespace Restaurant.API.Utility
{
    public enum OrderStatus
    {
        Pending,
        Confirmed, //PaymentReceived
        BeingCooked,
        ReadyForPickUp,
        Completed,
        Cancelled, //PaymentFailed
    }
}
