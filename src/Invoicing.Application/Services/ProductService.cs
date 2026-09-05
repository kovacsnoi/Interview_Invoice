namespace Invoicing.Application.Services;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Mapping;
using Invoicing.Application.Models;

public class ProductService : IProductService
{
  private readonly IUnitOfWork _unitOfWork;

  public ProductService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<IEnumerable<ProductDto>> GetAllAsync()
  {
    var products = await _unitOfWork.Products.GetAllAsync();
    return products.Select(p => p.ToDto());
  }

  public async Task<ProductDto?> GetByIdAsync(int id)
  {
    var product = await _unitOfWork.Products.GetByIdAsync(id);
    return product?.ToDto();
  }

  public async Task<ProductDto> CreateAsync(CreateProductDto dto)
  {
    var product = dto.ToEntity();
    await _unitOfWork.Products.AddAsync(product);
    await _unitOfWork.SaveChangesAsync();
    return product.ToDto();
  }
}