namespace Restaurant.API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IMenuItemRepository MenuItem { get; }
        ICategoryRepository Category { get; }
        IAuthRepository AuthRepository { get; }
        IShoppingCartRepository ShoppingCart { get; }
        ICartItemRepository CartItem { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        IBookingRepository Booking { get; }
        IUserRepository User { get; }

        Task SaveAsync();
    }
}
