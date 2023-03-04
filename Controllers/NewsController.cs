using Newtonsoft.Json;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
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
        // init with your API key
        var newsApiClient = new NewsApiClient("fda46bf2540743bab7ad46544eeb3dc0");
        var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
        {
            Q = "Technology",
            SortBy = SortBys.Popularity
        });
        var newsViewModels = ConvertArticlesToNewsViewModels(articlesResponse.Articles);
        return View(newsViewModels.Take(12));
    }
    public async Task<IActionResult> Search(string? keyword)
    {
        ViewBag.KeyWord = keyword;
        var newsApiClient = new NewsApiClient("fda46bf2540743bab7ad46544eeb3dc0");
        var articlesResponse = await newsApiClient.GetEverythingAsync(new EverythingRequest
        {
            Q = keyword,
            SortBy = SortBys.Popularity
        });
        var newsViewModels = ConvertArticlesToNewsViewModels(articlesResponse.Articles);
        return View(newsViewModels.Take(12));
    }
    private IList<NewsViewModel> ConvertArticlesToNewsViewModels(IList<NewsAPI.Models.Article> articles)
    {
        articles ??= new List<NewsAPI.Models.Article>();
        IList<NewsViewModel> viewModels = new List<NewsViewModel>();
        foreach (var article in articles)
        {
            viewModels.Add(new NewsViewModel
            {
                Author = article.Author,
                Title = article.Title,
                Description = article.Description,
                Url = article.Url,
                UrlToImage = article.UrlToImage,
                PublishedAt = article.PublishedAt.ToString(),
            });
        }
        return viewModels;
    }

}