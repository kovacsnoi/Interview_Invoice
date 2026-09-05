namespace Invoicing.Application.Interfaces;

using Invoicing.Application.Models;

public interface ICustomerService
{
  Task<IEnumerable<CustomerDto>> GetAllAsync();
  Task<CustomerDto?> GetByIdAsync(int id);
  Task<CustomerDto> CreateAsync(CreateCustomerDto dto);
}