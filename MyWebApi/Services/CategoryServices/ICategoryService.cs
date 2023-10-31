using MyWebApi.Models.CategoryModels;

namespace MyWebApi.Services.CategoryServices
{
    public interface ICategoryService
    {
        IEnumerable<CategoryVM> GetAll();
        CategoryVM GetById(int id);
        CategoryVM Create(CategoryModel category);
        CategoryVM Update(CategoryVM category);
        void Delete(int id);
    }
}
