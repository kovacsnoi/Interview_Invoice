namespace Invoicing.Application.Interfaces;

using Invoicing.Application.Models;

public interface IProductService
{
  Task<IEnumerable<ProductDto>> GetAllAsync();
  Task<ProductDto?> GetByIdAsync(int id);
  Task<ProductDto> CreateAsync(CreateProductDto dto);
}