using System.Diagnostics.CodeAnalysis;

namespace RecepiesByGirls.Data
{
    public class Recipe
    {
        [SetsRequiredMembers]
        public Recipe(string label, string url)
        {
            Label = label;
            Url = url;    
        }

        public required string Label { get; set; }
        public required string Url { get; set; }
    }
}
