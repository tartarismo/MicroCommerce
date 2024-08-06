using ProductsService.Models;

namespace ProductsService.Services
{
    public interface IProductService
    {
        void AddProduct(Product product);
    }
}