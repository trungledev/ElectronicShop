namespace ElectronicShop.Controllers;

public class CartsController : Controller
{
    private ApplicationDbContext _context;
    private string PathMessageView = "~/Views/Carts/_Message.cshtml";
    private readonly UserManager<ApplicationUser> _userManager;

    public CartsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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

        var messageViewModel = new CartMessageViewModel();
        //Find the product
        var productFound = _context.Products.Where(p => p.ProductId == id).FirstOrDefault();
        //User not logged
        if (!HttpContext.User.Identity!.IsAuthenticated)
        {
            Console.WriteLine("Hello, ");
            messageViewModel.Message = "Đăng nhập/ký để tiếp tục";
            messageViewModel.Success = -1;
            messageViewModel.ReturnUrl="~/Products/Detail?id=" + id;
            messageViewModel.IsSigned = false;
        }
        else if (productFound == null)
        {
            messageViewModel.Message = "Không tìm thấy sản phẩm";
            messageViewModel.Success = -1;
        }
        else
        {
            //Check product exist in cart
            var userIdCurrent = await GetCurrentUserIdAsync();
            var productCart = _context.Carts.Where(c => c.ProductId == id && c.UserId == userIdCurrent).FirstOrDefault();
            if (productCart == null)
            {
                _context.Carts.Add(new Cart()
                {
                    UserId = userIdCurrent,
                    ProductId = productFound.ProductId,
                    Quantity = quantity,
                    Total = quantity * productFound.Price
                });
                await _context.SaveChangesAsync();
                messageViewModel.Message = "Đã thêm vào giỏ hàng";
                messageViewModel.Success = 1;
            }
            else
            {
                productCart.Quantity += quantity;
                _context.Carts.Update(productCart);
                await _context.SaveChangesAsync();
                messageViewModel.Message = "Đã tăng thêm số lượng sản phẩm";
                messageViewModel.Success = 1;
            }
        }
        return PartialView(PathMessageView, messageViewModel);
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