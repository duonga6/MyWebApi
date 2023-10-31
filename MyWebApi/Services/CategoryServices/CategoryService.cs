using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models.CategoryModels;

namespace MyWebApi.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly MyDbContext _context;

        public CategoryService(MyDbContext context) { _context = context; }

        public CategoryVM Create(CategoryModel category)
        {
            var _category = new Category()
            {
                Name = category.Name,
                Description = category.Description,
            };

            _context.Categories.Add(_category);
            _context.SaveChanges();

            return new CategoryVM
            {
                Id = _category.Id,
                Name = category.Name,
                Description = category.Description,
            };
        }

        public void Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        public IEnumerable<CategoryVM> GetAll()
        {
            var categories = _context.Categories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
            }).ToList();

            return categories;
        }

        public CategoryVM GetById(int id)
        {
            var _category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (_category != null)
                return new CategoryVM
                {
                    Id = _category.Id,
                    Name = _category.Name,
                    Description = _category.Description,
                };

            return null;
        }

        public CategoryVM Update(CategoryVM category)
        {
            var _category = _context.Categories.FirstOrDefault(c => c.Id == category.Id);
            if (_category != null)
            {
                _category.Name = category.Name;
                _category.Description = category.Description;

                _context.SaveChanges();

                return new CategoryVM
                {
                    Id = _category.Id,
                    Name = category.Name,
                    Description = category.Description,
                };

            }

            return null;
        }
    }
}
