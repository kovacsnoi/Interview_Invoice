namespace Invoicing.Application.Services;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Mapping;
using Invoicing.Application.Models;

public class CustomerService : ICustomerService
{
  private readonly IUnitOfWork _unitOfWork;

  public CustomerService(IUnitOfWork unitOfWork)
  {
    _unitOfWork = unitOfWork;
  }

  public async Task<IEnumerable<CustomerDto>> GetAllAsync()
  {
    var customers = await _unitOfWork.Customers.GetAllAsync();
    return customers.Select(c => c.ToDto());
  }

  public async Task<CustomerDto?> GetByIdAsync(int id)
  {
    var customer = await _unitOfWork.Customers.GetByIdAsync(id);
    return customer?.ToDto();
  }

  public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
  {
    var customer = dto.ToEntity();
    await _unitOfWork.Customers.AddAsync(customer);
    await _unitOfWork.SaveChangesAsync();
    return customer.ToDto();
  }
}