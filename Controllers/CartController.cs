namespace ElectronicShop.Controllers;

public class CartController :Controller
{
    private ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _context = context;
    }
    [HttpGet]
    public async Task<string> GetCurrentUserIdAsync()
    { 
        ApplicationUser ApplicationUser = await GetCurrentUserAsync();
        return ApplicationUser.Id;
    }
    private  Task<ApplicationUser> GetCurrentUserAsync() =>
         _userManager.GetUserAsync( HttpContext.User);

    
    public async Task<IActionResult> Index()
    {
        var userId = await GetCurrentUserIdAsync();
        var products = _context.Carts.Where(x => x.UserId == userId);
        var gioHang = new List<CartViewModel>();
        foreach (var product in products)
        {
            var m = _context.Products.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
            if (m != null)
            {
                gioHang.Add(new CartViewModel
                {
                    Id = product.ProductId,
                    Name = m.Name,
                    Infomation = m.Information,
                    Quantity = product.Quantity,
                    Price = m.Price,
                    Total = product.Total
                });
            }
        }
        return View(gioHang);
    }
    [HttpGet]
    public async void EditCart(int id, int quantity, double price)
    {
        var userId = await GetCurrentUserIdAsync();
        var product = _context.Carts.Where(x => x.ProductId == id && x.UserId == userId).FirstOrDefault();
        if (product != null)
        {
            if (quantity == 0)
            {
                _context.Carts.Remove(product);
            }
            product.Quantity = quantity;
            product.Total = price * quantity;
            _context.SaveChanges();
        }
    }
}