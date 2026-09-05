namespace Invoicing.Application.Interfaces;

using Invoicing.Application.Models;

public interface IInvoiceDocumentGenerator
{
  byte[] GeneratePdf(InvoiceDto invoice);
}