namespace RecepiesByGirls.Models
{
    public class RecipeBySearch
    {
        public class RecipesObjModel
        {
            public List<RecipeModel>? hits { get; set; }
        }
        public class RecipeModel
        {
            public Recipe? recipe { get; set; }

        }
        public class Recipe
        {
            public string? url { get; set; }
            public string? label { get; set; }
            public List<Ingredient> ingredients { get; set; }

        }
        public class Ingredient
        {
            public string text { get; set; }
            public double quantity { get; set; }
            public string measure { get; set; }
            public string food { get; set; }
            public double weight { get; set; }
            public string foodCategory { get; set; }
            public string image { get; set; }
        }
    }
}
