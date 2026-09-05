namespace Invoicing.Api.Controllers;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
  private readonly ICustomerService _customerService;

  public CustomersController(ICustomerService customerService)
  {
    _customerService = customerService;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
  {
    return Ok(await _customerService.GetAllAsync());
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<CustomerDto>> GetById(int id)
  {
    var customer = await _customerService.GetByIdAsync(id);
    if (customer is null) return NotFound();
    return Ok(customer);
  }

  [HttpPost]
  public async Task<ActionResult<CustomerDto>> Create(CreateCustomerDto dto)
  {
    var customer = await _customerService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
  }
}