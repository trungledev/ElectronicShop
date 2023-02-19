namespace ElectronicShop.Controllers;

public class CartController : Controller
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
    private Task<ApplicationUser> GetCurrentUserAsync() =>
         _userManager.GetUserAsync(HttpContext.User);


    public async Task<IActionResult> Index()
    {
        var userId = await GetCurrentUserIdAsync();
        var products = _context.Carts.Where(x => x.UserId == userId).ToList();
        var cart = new List<CartViewModel>();
        foreach (var product in products)
        {
            var m = _context.Products.Where(x => x.ProductId == product.ProductId).FirstOrDefault();
            if (m != null)
            {
                cart.Add(new CartViewModel
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
        return View(cart);
    }
    [HttpPost]
    public async Task<IActionResult> AddToCart(int id, int quantity)
    {

        var messageViewModel = new MessageViewModel();
        //Find the product
        var productFound = _context.Products.Where(p => p.ProductId == id).FirstOrDefault();
        if (productFound == null)
        {
            messageViewModel.Message = "Khong tim thay san pham";
            messageViewModel.Success = -1;
            return PartialView("~/Views/Shared/_MessageModel.cshtml", messageViewModel);
        }
        else
        {
            _context.Carts.Add(new Cart(){
                UserId = await GetCurrentUserIdAsync(),
                ProductId = productFound.ProductId,
                Quantity = quantity,
                Total = quantity * productFound.Price
            });
            await _context.SaveChangesAsync();
            messageViewModel.Message = "Them vao gio";
            messageViewModel.Success = 1;
            return PartialView("~/Views/Shared/_MessageModel.cshtml", messageViewModel);
        }
    }
    [HttpGet]
    public async Task UpdateCart(int id, int quantity, double price)
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
            await _context.SaveChangesAsync();
        }
    }

}