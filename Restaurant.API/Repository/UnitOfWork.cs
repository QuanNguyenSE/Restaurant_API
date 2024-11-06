using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Restaurant.API.Data;
using Restaurant.API.Models;
using Restaurant.API.Repository.IRepository;

namespace Restaurant.API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public IMenuItemRepository MenuItem { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IAuthRepository AuthRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mapper = mapper;
            MenuItem = new MenuItemRepository(_db);
            Category = new CategoryRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            AuthRepository = new AuthRepository(_userManager, _roleManager, _configuration, _mapper);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
