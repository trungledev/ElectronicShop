namespace ElectronicShop.Services;

/*
    Sumary:
        Get Data when controller call
*/
public interface IServiceGetData<TModel> where TModel : class
{
    public List<TModel> GetModels();

}