using Newtonsoft.Json;
using System.IO;

namespace MvcMovie.Controllers;

public class NewsController : Controller
{
    private readonly string PATH_FILE_SAVE = "DataNews/DataNews.txt";
    private readonly string KEY_WORD = "electronic";
    public NewsController()
    {

    }
    public async Task<IActionResult> Index()
    {
        return await Search(null);
    }
    public async Task<IActionResult> Search(string? keyword)
    {
        keyword ??= "technology";
        ViewBag.KeyWord = keyword;
        string urlApi = "https://newsapi.org/v2/everything?" + "q=" + keyword + "&apiKey=fda46bf2540743bab7ad46544eeb3dc0";
        IList<NewsViewModel> newsViewModels = new List<NewsViewModel>();
        var dataApi = new DataNewsApi();
        try
        {
            dataApi = await GetApiAndConvertToDataNews(urlApi);
        }
        catch (HttpRequestException exception)
        {
            try
            {
                dataApi = ReadFileAndConvertToDataNews(PATH_FILE_SAVE);
                if (dataApi == null)
                {
                    //False Get API response
                    string errorMessage = "Failed to read and get API response: " + exception.Message;
                    ViewBag.ErrorMessage = errorMessage;
                    return View(newsViewModels);
                }
            }
            catch (FileNotFoundException ex)
            {
                dataApi = null;
                ViewBag.ErrorMessage = ex.Message;
                System.IO.Directory.CreateDirectory("DataNews");
                System.IO.File.WriteAllText(PATH_FILE_SAVE,"");
            }
        }
        catch (Exception ex)
        {
            //Throw exception diferent : 
            string errorMessage = "Failed with error message: " + ex.Message;
            ViewBag.ErrorMessage = errorMessage;
            return View(newsViewModels);

        }
        if (dataApi == null || dataApi.articles == null)
        {
            ViewBag.ErrorMessage = "Can't not convert dataApi to articles";
            return View(newsViewModels);
        }
        var listRandomArticles = GetRandomArticles(12, dataApi.articles);
        newsViewModels = ConvertArticlesToNewsViewModels(listRandomArticles);
        ViewBag.TotalResults = dataApi.articles.Count();
        return View(newsViewModels);
    }
    private async Task<string> GetJsonStringFromUrlAPIAsync(string urlApi)
    {
        var data = new DataNewsApi();
        string body = string.Empty;
        var client = new HttpClient();
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(urlApi)
        };
        using (var response = await client.SendAsync(request))
        {
            response.EnsureSuccessStatusCode();
            body = await response.Content.ReadAsStringAsync();
            data = JsonConvert.DeserializeObject<DataNewsApi>(body);
            if (data == null || data.articles == null)
                return string.Empty;
            return body;
        }
    }
    private async Task WriteFileAsync(string path, string data)
    {
        using (StreamWriter writer = new StreamWriter(path))
        {
            await writer.WriteLineAsync(data);
        }
        if (new FileInfo(path).Length == 0)
            throw new Exception("Write failed");
    }
    private string ReadAllFile(string path)
    {
        return System.IO.File.ReadAllText(path);
    }
    private DataNewsApi? ConvertFileNewsApiToData(string data)
    {
        if (string.IsNullOrEmpty(data))
            return null;
        return JsonConvert.DeserializeObject<DataNewsApi>(data);
    }
    private IList<Articles> GetRandomArticles(int quantity, List<Articles> listArticles)
    {
        Random rand = new Random();
        IList<Articles> newsApiList = new List<Articles>();
        for (int i = 0; i < quantity; i++)
        {
            newsApiList.Add(listArticles.ElementAt(rand.Next(listArticles.Count())));
        }
        return newsApiList;
    }
    private bool IsEmptyFile(string pathFile)
    {
        return new FileInfo(pathFile).Length == 0;
    }
    private IList<NewsViewModel> ConvertArticlesToNewsViewModels(IList<Articles> articles)
    {
        IList<NewsViewModel> viewModels = new List<NewsViewModel>();
        foreach (var article in articles)
        {
            viewModels.Add(new NewsViewModel
            {
                Author = article.author,
                Title = article.title,
                Description = article.description,
                Url = article.url,
                UrlToImage = article.urlToImage,
                PublishedAt = article.publishedAt
            });
        }
        return viewModels;
    }
    private DataNewsApi? ReadFileAndConvertToDataNews(string path)
    {
        if (IsEmptyFile(path))
            return null;
        string dataFile = ReadAllFile(path);
        return ConvertFileNewsApiToData(dataFile);
    }
    private async Task<DataNewsApi?> GetApiAndConvertToDataNews(string urlApi)
    {
        var jsonDataString = await GetJsonStringFromUrlAPIAsync(urlApi);
        return JsonConvert.DeserializeObject<DataNewsApi>(jsonDataString);
    }
}