using Restaurant.API.Models;

namespace Restaurant.API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IMenuItemRepository MenuItem { get; }
        ICategoryRepository Category { get; }
        IAuthRepository AuthRepository { get;}
        IShoppingCartRepository ShoppingCart {  get; }
        Task SaveAsync();
    }
}
