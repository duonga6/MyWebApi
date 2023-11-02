using MyWebApi.Models;
using MyWebApi.Models.ProductModels;

namespace MyWebApi.Services.ProductServices
{
    public interface IProductService
    {
        PaginatedList<ProductVM> Get(string? search, double? from, double? to, string? sortBy, int page = 1);
        ProductVM GetById(int id);
        ProductVM Create(ProductModel product);
        ProductVM Update(ProductVM product);
        void Delete(int id);
    }
}
