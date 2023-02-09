namespace ElectronicShop.Services;

public class ServiceGetData<TModel> : IServiceGetData<TModel> where TModel : class
{
    private readonly ApplicationDbContext _context;
    public ServiceGetData(ApplicationDbContext context)
    {
        _context = context;
    }
    public List<TModel> GetModels()
    {
        var list = _context.Set<TModel>().ToList();
        return list;
    }
   
}