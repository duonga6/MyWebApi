using Microsoft.EntityFrameworkCore;
using MyWebApi.Data;
using MyWebApi.Models;
using MyWebApi.Models.CategoryModels;
using MyWebApi.Models.ProductModels;

namespace MyWebApi.Services.ProductServices
{
    public class ProductSerivce : IProductService
    {
        private readonly MyDbContext _context;
        public int PAGE_SIZE { get; set; } = 10;
        public ProductSerivce(MyDbContext context)
        {
            _context = context;
        }

        public PaginatedList<ProductVM> Get(string? search, double? from, double? to, string? sortBy, int page = 1)
        {
            var allProducts = _context.Products.Include(p => p.Category).AsQueryable();

            #region Search
            if (search != null)
            {
                allProducts = allProducts.Where(p => p.Name.Contains(search));
            }
            #endregion

            #region Filter
            if (from.HasValue)
            {
                allProducts = allProducts.Where(p => p.Price >= from);
            }

            if (to.HasValue)
            {
                allProducts = allProducts.Where(p => p.Price <= to);
            }

            switch (sortBy)
            {
                case "name_asc":
                    allProducts = allProducts.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    allProducts = allProducts.OrderByDescending(p => p.Name);
                    break;
                case "price_asc":
                    allProducts = allProducts.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    allProducts = allProducts.OrderByDescending(p => p.Price);
                    break;
            }

            #endregion

            #region Paging
            var allProductVM = allProducts.Select(rs => new ProductVM
            {
                Id = rs.Id,
                Name = rs.Name,
                Price = rs.Price,
                Description = rs.Description,
                Category = rs.Category != null ? new CategoryVM
                {
                    Id = rs.Category.Id,
                    Name = rs.Category.Name,
                    Description = rs.Category.Description
                } : null
            });

            var result = PaginatedList<ProductVM>.Create(allProductVM, page, PAGE_SIZE);
            #endregion

            return result;
        }

        public ProductVM GetById(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
            if (product == null)
                return null;

            return new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category != null ? new CategoryVM
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description
                } : null
            };
        }

        public ProductVM Create(ProductModel product)
        {
            var _product = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
            };

            if (product.CategoryId != null)
            {
                var category = _context.Categories.FirstOrDefault(c => c.Id == product.CategoryId) ?? throw new Exception("Mã danh mục không tồn tại");
                _product.Category = category;
            }

            _context.Products.Add(_product);
            _context.SaveChanges();

            return new ProductVM
            {
                Id = _product.Id,
                Name = _product.Name,
                Description = _product.Description,
                Price = _product.Price,
                Category = _product.Category != null ? new CategoryVM
                {
                    Id = _product.Category.Id,
                    Name = _product.Category.Name,
                    Description = _product.Category.Description
                } : null
            };
        }

        public ProductVM Update(ProductVM product)
        {
            var _product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == product.Id);
            if (_product == null)
                return null;

            _product.Name = product.Name;
            _product.Description = product.Description;
            _product.Price = product.Price;

            if (product.Category == null)
                _product.Category = null;
            else
            {
                if (_product.CategoryId != product.Category.Id)
                {
                    var category = _context.Categories.FirstOrDefault(c => c.Id == product.Category.Id) ?? throw new Exception("Mã danh mục không tồn tại");
                    _product.Category = category;
                }    
            }

            _context.SaveChanges();

            return new ProductVM
            {
                Id = _product.Id,
                Name = _product.Name,
                Description = _product.Description,
                Price = _product.Price,
                Category = _product.Category != null ? new CategoryVM
                {
                    Id = _product.Category.Id,
                    Name = _product.Category.Name,
                    Description = _product.Category.Description
                } : null
            };

        }

        public void Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }    
        }
    }
}
