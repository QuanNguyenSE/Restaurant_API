namespace Restaurant.API.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IMenuItemRepository MenuItem { get; }
        Task SaveAsync();
    }
}
