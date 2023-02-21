using System.Text.RegularExpressions;

namespace ElectronicShop.Controllers;

public class ReviewsController : Controller
{
    private ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly string _pathMessageView = "~/Views/Reviews/_Message.cshtml";

    public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    [HttpGet]
    public async Task<IActionResult> Index(int productId)
    {
        //Data cua ViewData[]
        var starDetail = GetAllNumberStar(productId, _context);
        var reviewViewModel = new ReviewViewModel();

        var reviewsViewModels = new List<ReviewViewModel>();
        var reviews = _context.Reviews.Where(x => x.ProductId == productId).ToList();
        if (reviews.Count() > 0)
        {
            foreach (var review in reviews)
            {
                reviewsViewModels.Add(new ReviewViewModel
                {
                    Title = review.Title,
                    Content = review.Content ?? string.Empty,
                    // Time = review.Time,
                    AverageStar = GetAverageStar(review.ProductId),
                    NumberOfStar = review.NumberOfStar,
                    Author = await GetNameUserByIdAsync(review.UserId)
                });
                Console.WriteLine("Show review");
            }
        }
        else
        {
            reviewViewModel = null;
            Console.WriteLine("Lenght = " + reviews.Count());
        }
        if (User.Identity!.IsAuthenticated)
        {
            //Khi nguoi dung da dang nhap => show Review cua nguoi dung dong dau tien
            var userId = await GetCurrentUserId();
            var reviewOfUser = _context.Reviews.Where(x => x.ProductId == productId && x.UserId == userId).FirstOrDefault();
            if (reviewOfUser != null)
            {
                reviewViewModel = new ReviewViewModel()
                {
                    ProductId = productId,
                    NumberOfStar = reviewOfUser.NumberOfStar,
                    Title = reviewOfUser.Title,
                    Content = reviewOfUser.Content ?? string.Empty,
                    // Time = reviewOfUser.NgayDang,
                    Author = await GetNameUserByIdAsync(reviewOfUser.UserId)
                };
                //Day reivew qua ViewData[];
                ViewData["ReviewOfUser"] = reviewViewModel;
            }
            else
            {
                ViewData["ReviewOfUser"] = null;
            }

        }
        else
            ViewData["ReviewOfUser"] = null;


        ViewData["StarDetail"] = starDetail;


        return PartialView("_Reviews", reviewsViewModels);
    }

    private double GetAverageStar(int productId)
    {
        var reviews = _context.Reviews.Where(r => r.ProductId == productId).ToList();
        var averageStar = reviews.Average(x => x.NumberOfStar);
        return averageStar;
    }

    private int GetQuantityReview(int productId)
    {
        var reviewsFound = _context.Reviews.Where(x => x.ProductId == productId).ToArray();
        return reviewsFound.Length;
    }

    private async Task<string> GetNameUserByIdAsync(string userId)
    {
        var userFound = await _userManager.FindByIdAsync(userId);
        if (userFound == null)
            return string.Empty;
        else
            return userFound.FullName;

    }

    public static ReviewViewModel GetAllNumberStar(int id, ApplicationDbContext context)
    {
        var _context = context;
        var reviewsViewModel = new List<ReviewViewModel>();
        var reviewViewModel = new ReviewViewModel();
        var reviews = _context.Reviews.Where(x => x.ProductId == id);
        if (reviews != null)
        {
            var soReview = reviews.Count();
            IDictionary<string, int> datailStar = new Dictionary<string, int>();
            var oneStar = from review in reviews
                               where review.NumberOfStar == 1
                               select review;
            var twoStar = from review in reviews
                               where review.NumberOfStar == 2
                               select review;
            var threeStar = from review in reviews
                              where review.NumberOfStar == 3
                              select review;
            var fourStar = from review in reviews
                               where review.NumberOfStar == 4
                               select review;
            var fiveStar = from review in reviews
                               where review.NumberOfStar == 5
                               select review;
            var quantityReviewOneStar = oneStar.Count();
            var quantityReviewTwoStar = twoStar.Count();
            var quantityReviewThreeStar = threeStar.Count();
            var quantityReviewFourStar = fourStar.Count();
            var quantityReviewFiveStar = fiveStar.Count();


            datailStar.Add("oneStar", quantityReviewOneStar);
            datailStar.Add("twoStar", quantityReviewTwoStar);
            datailStar.Add("threeStar", quantityReviewThreeStar);
            datailStar.Add("fourStar", quantityReviewFourStar);
            datailStar.Add("fiveStar", quantityReviewFiveStar);

            var quantityAllReview = (quantityReviewOneStar + quantityReviewTwoStar + quantityReviewThreeStar + quantityReviewFourStar + quantityReviewFiveStar);
            double averageStar = 0;
            if (quantityAllReview > 0)
                averageStar = (quantityReviewOneStar * 1 + quantityReviewTwoStar * 2 + quantityReviewThreeStar * 3 + quantityReviewFourStar * 4 + quantityReviewFiveStar * 5) / (double)quantityAllReview;

            reviewViewModel = new ReviewViewModel
            {
                QuantityAllReview = quantityAllReview,
                AverageStar = averageStar,
                DetailStars = datailStar
            };

        }
        else
        {
            //Khong Tim Thay San Pham
        }
        return reviewViewModel;
    }

    [HttpGet]
    public async Task<IActionResult> CreateOrUpdateAsync(int productId)
    {
        const string PATH_VIEW = "_CreateOrUpdate";
        var messageModel = new ReviewMessageViewModel();
        var reviewViewModel = new ReviewViewModel();
        if (!User.Identity!.IsAuthenticated)
        {
            messageModel.IsSigned = false;
            messageModel.Success = -1;
            messageModel.ProductId = productId;
            messageModel.Message = "Đăng nhập/ký để tiếp tục";
            messageModel.ReturnUrl = "~/Products/Detail?id=" + productId;
            return PartialView(_pathMessageView, messageModel);
        }
        else
        {
            messageModel.IsSigned = true;
            var idApplicationUser = await GetCurrentUserId();
            var reviewInData = _context.Reviews.Where(x => x.UserId == idApplicationUser && x.ProductId == productId).FirstOrDefault();
            if (reviewInData == null)
            {
                //Chua tao review
                reviewViewModel.ProductId = productId;
                return PartialView(PATH_VIEW, reviewViewModel);

            }
            else
            {
                //Khi nguoi dung da dang nhap => show Review cua nguoi dung dong dau tien
                var userId = await GetCurrentUserId();
                var reviewOfUser = _context.Reviews.Where(x => x.ProductId == productId && x.UserId == userId).FirstOrDefault();
                if (reviewOfUser != null)
                {
                    reviewViewModel = new ReviewViewModel()
                    {
                        ProductId = productId,
                        NumberOfStar = reviewOfUser.NumberOfStar,
                        Title = reviewOfUser.Title,
                        Content = reviewOfUser.Content ?? string.Empty,
                        // Time = reviewOfUser.NgayDang,
                        Author = await GetNameUserByIdAsync(reviewOfUser.UserId)
                    };
                }
                return PartialView(PATH_VIEW, reviewViewModel);
            }
        }


    }
    [HttpPost]
    public async Task<IActionResult> CreateOrUpdateAsync([Bind(include: "ProductId,NumberOfStar,Title,Content")] ReviewViewModel input)
    {
        Console.WriteLine("number of stars :" + input.NumberOfStar);
        var messageModel = new ReviewMessageViewModel();
        if (!ModelState.IsValid)
        {
            messageModel.Success = -1;
            messageModel.Message = "Vui lòng điền đầy đủ thông tin";
            return PartialView(_pathMessageView, messageModel);
        }
        else
        {
            var reviewViewModel = new ReviewViewModel();
            var userId = await GetCurrentUserId();
            var review = _context.Reviews.Where(x => x.UserId == userId && x.ProductId == input.ProductId).FirstOrDefault();
            if (review != null)
            {
                review.NumberOfStar = input.NumberOfStar;
                review.Title = input.Title;
                review.Content = input.Content;

                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
                messageModel.Success = 1;
                messageModel.Message = "Thao tác thành công";
                return PartialView(_pathMessageView, messageModel);
            }
            else
            {
                try
                {
                    _context.Reviews.Add(new Review
                    {
                        ProductId = input.ProductId,
                        UserId = userId,
                        Title = input.Title,
                        Content = input.Content,
                        // NgayDang = DateTime.Now,
                        NumberOfStar = input.NumberOfStar

                    });
                    await _context.SaveChangesAsync();
                    messageModel.Success = 1;
                    messageModel.Message = "Đã thêm review";
                    Console.WriteLine("Them Review");
                    return PartialView(_pathMessageView, messageModel);

                }
                catch (Exception ex)
                {

                    messageModel.Success = -1;
                    messageModel.Message = "Đã có lỗi xảy ra : " + ex.Message;
                    Console.WriteLine("Loi them Review");
                    return PartialView(_pathMessageView, messageModel);
                }
            }
        }

    }
    [HttpGet]
    public IActionResult Remove(int productId)
    {
        var messageModel = new RemoveReviewViewModel();
        string returnUrl = "~/Products/Detail?id=" + productId;
        messageModel.ProductId = productId;
        messageModel.Message = " Xác nhận xóa";
        messageModel.ReturnUrl = returnUrl;
        return PartialView("~/Views/Reviews/_Remove.cshtml", messageModel);
    }
    [HttpPost]
    public async Task<IActionResult> RemoveAsync([Bind(include: "ProductId,ReturnUrl")] RemoveReviewViewModel removeModel)
    {
        var idApplicationUser = await GetCurrentUserId();
        var review = _context.Reviews.Where(x => x.ProductId == removeModel.ProductId && x.UserId == idApplicationUser).FirstOrDefault();
        if (review != null)
        {
            try
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
                return LocalRedirect(removeModel.ReturnUrl);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("ex: " + ex.Message);
                return LocalRedirect(removeModel.ReturnUrl);
            }
        }
        else
        {
            return NotFound();
        }

    }
    [HttpGet]
    public async Task<IActionResult> IsReviewed(int productId)
    {
        var idApplicationUser = await GetCurrentUserId();
        var reviewInData = _context.Reviews.Where(x => x.UserId == idApplicationUser && x.ProductId == productId).FirstOrDefault();
        if (reviewInData == null)
        {
            //Chua tao review
            return Json(0);
        }
        else
        {
            //Da tao review
            return Json(1);

        }
    }
    private Task<ApplicationUser> GetCurrentUserAsync() =>
        _userManager.GetUserAsync(HttpContext.User);

    [HttpGet]
    public async Task<string> GetCurrentUserId()
    {
        ApplicationUser ApplicationUser = await GetCurrentUserAsync();
        return ApplicationUser.Id;
    }
}