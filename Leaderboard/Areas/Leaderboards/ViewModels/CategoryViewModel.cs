using System.Collections.Generic;
using Leaderboard.Areas.Leaderboards.Models;

namespace Leaderboard.Areas.Leaderboards.ViewModels
{
    public class CategoryViewModel
    {
        public string Name { get; set; }

        public CategoryViewModel(Category category)
        {
            Name = category.Name;
        }

        public static IEnumerable<CategoryViewModel> Create(IEnumerable<Category> categories)
        {
            foreach (var category in categories)
                yield return new CategoryViewModel(category);
        }
    }
}
