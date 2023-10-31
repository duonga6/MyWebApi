using MyWebApi.Data;
using MyWebApi.Models.CategoryModels;

namespace MyWebApi.Services.CategoryServices
{
    public class CategoryServiceInMemory : ICategoryService
    {
        static List<Category> Categories = new List<Category>()
        {
            new Category
            {
                Id = 1,
                Name = "Test",
                Description = "Test",
            }
        };
        public CategoryVM Create(CategoryModel category)
        {
            var _category = new Category
            {
                Id = Categories.Max(c => c.Id) + 1,
                Name = category.Name,
                Description = category.Description,
            };

            Categories.Add(_category);

            return new CategoryVM
            {
                Id = _category.Id,
                Name = _category.Name,
                Description = _category.Description,
            };
        }

        public void Delete(int id)
        {
            var category = Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                Categories.Remove(category);
            }
        }

        public IEnumerable<CategoryVM> GetAll()
        {
            return Categories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();
        }

        public CategoryVM GetById(int id)
        {
            var category = Categories.FirstOrDefault(c =>  id == c.Id);
            if (category != null)
            {
                return new CategoryVM
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                };
            }

            return null;
        }

        public CategoryVM Update(CategoryVM category)
        {
            var _category = Categories.FirstOrDefault(c => c.Id == category.Id);
            if (_category != null)
            {
                _category.Name = category.Name;
                _category.Description = category.Description;
            }

            return new CategoryVM
            {
                Id = _category.Id,
                Name = category.Name,
                Description = category.Description,
            };
        }
    }
}
