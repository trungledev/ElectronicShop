using System.Collections;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ElectronicShop.Controllers;

/*
    Sumary: 
       
*/
// [Authorize(Roles ="Admin")]
public class ProductsController : CRUDGeneric<Product, ProductViewModel, int>
{
    private ElectronicShop.Models.MessageViewModel _errorModel;
    private ViewData? _viewData;
    public ProductsController(ApplicationDbContext context) : base(context, "/Views/Products/")
    {
        _errorModel = new ElectronicShop.Models.MessageViewModel { Message = "" };
    }
    protected override string GetNamePage() => "Product";
    protected override string GetControllerName() => "Products";
    protected override int GetIdModel(Product model) => model.ProductId;

    protected override Product BuildModel(ProductViewModel viewModel)
    {
        if (viewModel != null)
        {
            Product product = new Product()
            {
                ProductId = viewModel.ProductId,
                Name = viewModel.NameProduct!,
                Information = viewModel.Information!,
                Price = viewModel.Price,
                Quantity = viewModel.Quantity,
                
                CategoryId = viewModel.CategoryId,
                ProducerId = viewModel.ProducerId,
                StatusId = viewModel.StatusId
            };
            return product;
        }
        else
        {
            return null!;
        }
    }
    protected override ProductViewModel BuildViewModel(Product? model)
    {

        ProductViewModel viewModel = new ProductViewModel();
        if (model != null)
        {
            viewModel.ProductId = model.ProductId;
            viewModel.NameProduct = model.Name;
            viewModel.Information = model.Information;
            viewModel.Price = model.Price;
            viewModel.Quantity = model.Quantity;

            viewModel.CategoryId = model.CategoryId;
            viewModel.ProducerId = model.ProducerId;
            viewModel.StatusId = model.StatusId;

            viewModel.Category = GetNameTypeById(model.CategoryId);
            viewModel.Status = GetNameStatusById(model.StatusId);
            viewModel.Producer = GetNameProducerById(model.ProducerId);

            //Get all data Type
            viewModel.Categories = GetTypeProducts();
            //Get all data Status
            viewModel.Statuses = GetStatusProducts();
            //Get all data Producer
            viewModel.Producers = GetProducerProducts();
        }
        else
        {
            viewModel.Categories = GetTypeProducts();
            //Get all data Status
            viewModel.Statuses = GetStatusProducts();
            //Get all data Producer
            viewModel.Producers = GetProducerProducts();
        }
        return viewModel;
    }
    private string GetNameTypeById(int id)
    {
        return _context.Categories.Find(id)!.Name;
    }
    private string GetNameStatusById(int id)
    {
        return _context.Statuses.Find(id)!.Name;
    }
    private string GetNameProducerById(int id)
    {
        return _context.Producers.Find(id)!.Name;
    }
    private IEnumerable<SelectListItem> GetProducerProducts()
    {
        return _context.Producers.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    private IEnumerable<SelectListItem> GetStatusProducts()
    {
        return _context.Statuses.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();

    }

    private IEnumerable<SelectListItem> GetTypeProducts()
    {
        return _context.Categories.Select(x => new SelectListItem()
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }).ToList();
    }

    protected override IEnumerable<ProductViewModel> BuildListViewModel(IEnumerable<Product> models)
    {
        IList<ProductViewModel> viewData = new List<ProductViewModel>();
        foreach (var product in models)
        {
            viewData.Add(new ProductViewModel()
            {
                ProductId = product.ProductId,
                NameProduct = product.Name,
                Information = product.Information,
                Price = product.Price,
                Quantity = product.Quantity,

                CategoryId = product.CategoryId,
                ProducerId = product.ProducerId,
                StatusId = product.StatusId,

                Category = GetNameTypeById(product.CategoryId),
                Status = GetNameStatusById(product.StatusId),
                Producer = GetNameProducerById(product.ProducerId),

                Categories = GetTypeProducts(),
                Statuses = GetStatusProducts(),
                Producers = GetProducerProducts()
            });
        }
        return viewData;
    }
    [HttpGet]
    public IActionResult ShowProductByType(string valueOfProperty, int numberic)
    {
        var type = _context.Categories.Where(x => x.Name == valueOfProperty).FirstOrDefault();
        int id = 0;
        if(type == null)
        {
            return NotFound();
        }
        else
        {
            id = type.Id;
        }
        var products = _context.Products.Where(x => x.CategoryId == id).ToList();

        IEnumerable<ProductViewModel> viewData = BuildListViewModel(products).Take(numberic);
        string? viewPathShowProduct = _viewPath + "/ShowProduct";

        return PartialView(viewPathShowProduct,viewData);
    }

}