using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpApp.Core.Interfaces
{
    public interface ICategoriesService
    {
        Task<IReadOnlyCollection<Category>> GetCategories();
        Task<Category> GetCategory(int id);
        Task<Category> CreateCategory(CreateCategoryRequest request);
        Task<Category> UpdateCategory(int id, UpdateCategoryRequest request);
    }
}

