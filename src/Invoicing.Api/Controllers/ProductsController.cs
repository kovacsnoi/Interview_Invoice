namespace Invoicing.Api.Controllers;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
  private readonly IProductService _productService;

  public ProductsController(IProductService productService)
  {
    _productService = productService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
  {
    return Ok(await _productService.GetAllAsync());
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<ProductDto>> GetById(int id)
  {
    var product = await _productService.GetByIdAsync(id);
    if (product is null) return NotFound();
    return Ok(product);
  }

  [HttpPost]
  public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
  {
    var product = await _productService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
  }
}