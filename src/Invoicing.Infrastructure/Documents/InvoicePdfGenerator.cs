namespace Invoicing.Infrastructure.Documents;

using Invoicing.Application.Interfaces;
using Invoicing.Application.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class InvoicePdfGenerator : IInvoiceDocumentGenerator
{
  public byte[] GeneratePdf(InvoiceDto invoice)
  {
    var document = Document.Create(container =>
    {
      container.Page(page =>
      {
        page.Size(PageSizes.A4);
        page.Margin(40);
        page.DefaultTextStyle(x => x.FontSize(11));

        page.Header().Column(column =>
        {
          column.Item().Text("SZÁMLA").FontSize(20).Bold();
          column.Item().PaddingTop(10).Text($"Ügyfél: {invoice.CustomerName}");
          column.Item().Text($"Dátum: {invoice.Date:yyyy-MM-dd}");
          column.Item().Text($"Rendelés azonosító: {invoice.OrderId}");
        });

        page.Content().PaddingTop(20).Table(table =>
        {
          table.ColumnsDefinition(columns =>
          {
            columns.RelativeColumn(4);
            columns.RelativeColumn(1.5f);
            columns.RelativeColumn(2);
            columns.RelativeColumn(2); 
          });

          table.Header(header =>
          {
            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5).Text("Termék").Bold();
            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5).AlignRight().Text("Mennyiség").Bold();
            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5).AlignRight().Text("Egységár").Bold();
            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).PaddingVertical(5).AlignRight().Text("Összesen").Bold();
          });

          foreach (var item in invoice.Items)
          {
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(4).Text(item.FormattedProductName);
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(4).AlignRight().Text(item.Quantity.ToString());
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(4).AlignRight().Text(item.UnitPrice.ToString("0.00"));
            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3).PaddingVertical(4).AlignRight().Text(item.LineTotal.ToString("0.00"));
          }
        });

        page.Footer().AlignRight().PaddingTop(10).Text(text =>
        {
          text.Span("Végösszeg: ").Bold();
          text.Span($"{invoice.TotalAmount:0.00}").Bold().FontSize(14);
        });
      });
    });

    return document.GeneratePdf();
  }
}