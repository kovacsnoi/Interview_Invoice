namespace Invoicing.Api.Controllers;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
  private readonly IOrderService _orderService;
  private readonly IInvoiceDocumentGenerator _invoiceGenerator;

  public OrderController(IOrderService orderService, IInvoiceDocumentGenerator invoiceGenerator)
  {
    _orderService = orderService;
    _invoiceGenerator = invoiceGenerator;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
  {
    return Ok(await _orderService.GetAllAsync());
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<OrderDto>> GetById(int id)
  {
    var order = await _orderService.GetByIdAsync(id);
    if (order is null) return NotFound();
    return Ok(order);
  }

  [HttpPost]
  public async Task<IActionResult> Create(CreateOrderDto dto)
  {
    try
    {
      var order = await _orderService.CreateOrderAsync(dto);
      return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
    catch (ArgumentException ex)
    {
      return BadRequest(ex.Message);
    }
    catch (InvalidOperationException ex)
    {
      return BadRequest(ex.Message);
    }
  }

  [HttpGet("{id}/invoice")]
  public async Task<IActionResult> GetInvoice(int id)
  {
    var invoice = await _orderService.GenerateInvoiceAsync(id);
    if (invoice is null) return NotFound();

    var pdfBytes = _invoiceGenerator.GeneratePdf(invoice);
    return File(pdfBytes, "application/pdf", $"invoice_{id}.pdf");
  }
}