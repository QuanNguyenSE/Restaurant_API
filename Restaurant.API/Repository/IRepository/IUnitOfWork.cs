namespace Restaurant.API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IMenuItemRepository MenuItem { get; }
        ICategoryRepository Category { get; }
        IAuthRepository AuthRepository { get;}
        Task SaveAsync();
    }
}
