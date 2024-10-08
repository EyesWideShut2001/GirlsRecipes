using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RecepiesByGirls.Data;
using RecepiesByGirls.Models;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
using static RecepiesByGirls.Models.RecipeBySearch;

namespace RecepiesByGirls.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private static readonly ConcurrentDictionary<string, List<Ingredient>> _data = new ConcurrentDictionary<string, List<Ingredient>>();

        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {
            _configuration = configuration;
            _logger = logger;            
        }
       
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult RecipeBySearch()
        {
            return View();
        }

        public IActionResult User()
        {
            return View();
        }
      
        [HttpPost]
        public IActionResult UserCongrats()
        {
            return View();
        }
        
        public IActionResult FavouritesRecipes()
        {
            RecipeDBController.Init();
            ViewBag.Recipes = RecipeDBController.GetRecipes();
            return View();
        }

        public async Task<IActionResult> Details(string id)
        {
            if (_data.TryGetValue(id, out var ingredients))
            {
                return View(ingredients);
            }
            else
            {
                return View(new List<Ingredient>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveFavouriteRecipe([FromBody] RecipeDto recipe)
        {
            if (recipe == null || string.IsNullOrEmpty(recipe.Label))
            {
                ViewBag.ErrorMessage = "The label does not exist.";
                _logger.LogError("Label is missing.");
                return View("SaveFavouriteRecipe");
            }

            if (string.IsNullOrEmpty(recipe.Url))
            {
                ViewBag.ErrorMessage = "The url is missing or do not exist.";
                _logger.LogError("Url is missing.");
                return View("SaveFavouriteRecipe");
            }
            _logger.LogInformation("label = " + recipe.Label + "url = " + recipe.Url);
            RecipeDBController.Init();
            RecipeDBController.SaveRecipe(recipe.Label, recipe.Url);
            return View("SaveFavouriteRecipe");
        }

        [HttpPost]
        public async Task<IActionResult> RecipeSearch(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                ViewBag.ErrorMessage = "Please enter a recipe to search for.";
                return View("SearchResults");
            }

            var apiKey = _configuration["EdamamApiKey"];
            var appId = _configuration["EdamamAppId"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(appId))
            {
                _logger.LogError("API key or App ID is missing.");
                ViewBag.ErrorMessage = "Configuration error. Please contact the administrator.";
                return View("SearchResults");
            }

            HttpClient client = new()
            {
                BaseAddress = new Uri(Constants.EDAMAM_URI_RECIPES_V2)
            };
            client.DefaultRequestHeaders.Add("Accept", Constants.APP_JSON);
            client.DefaultRequestHeaders.Add("Accept-Language", Constants.EN);

            try
            {
                var response = await client.GetAsync($"?type=public&q={searchQuery}&app_id={appId}&app_key={apiKey}");
                if (response.IsSuccessStatusCode)
                {
                    var contentAsString = await response.Content.ReadAsStringAsync();
                    var recipesObjModel = JsonConvert.DeserializeObject<RecipeBySearch.RecipesObjModel>(contentAsString);
                    if (recipesObjModel?.hits == null || recipesObjModel.hits.Count == 0)
                    {
                        ViewBag.ErrorMessage = "No recipes found.";
                    }
                    foreach(var recipe in recipesObjModel.hits)
                    {
                        _data[recipe.recipe.label] = recipe.recipe.ingredients;
                    }
                    return View("SearchResults", recipesObjModel);
                }
                else
                {
                    _logger.LogError($"API request failed with status code: {response.StatusCode}");
                    ViewBag.ErrorMessage = "Error retrieving recipes. Please try again later.";
                    return View("SearchResults");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching recipes.");
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                return View("SearchResults");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class RecipeDto
    {
        public string Label { get; set; }
        public string Url { get; set; }
    }

}
