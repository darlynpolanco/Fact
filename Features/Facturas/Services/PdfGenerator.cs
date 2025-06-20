using Fact.Data.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Fact.Features.Facturas.Services
{
    public class PdfGenerator : IPdfGenerator
    {
        public byte[] GenerateFacturaPdf(Factura factura)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Calibri));

                    page.Header()
                        .Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("FACTURANDO COMPANY")
                                    .Bold().FontSize(16).FontColor(Colors.Blue.Darken3);

                                column.Item().Text("Calle Principal, 123");
                                column.Item().Text("Tel: +123 456 7890");
                                column.Item().Text("Email: info@facturando.com");
                            });

                            row.RelativeItem().AlignRight().Column(column =>
                            {
                                column.Item().Text($"Factura #{factura.Id}")
                                    .Bold().FontSize(18).FontColor(Colors.Black);

                                column.Item().Text($"Fecha: {factura.Fecha:dd/MM/yyyy}");
                                column.Item().Text($"Vencimiento: {factura.Fecha.AddDays(15):dd/MM/yyyy}");
                                column.Item().Text($"Referencia: PAGO-{factura.Id}");
                            });
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Información del cliente
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Cliente:").Bold();
                                    col.Item().Text(factura.Cliente.Nombre);
                                    col.Item().Text(factura.Cliente.Direccion);
                                    col.Item().Text($"Email: {factura.Cliente.Email}");
                                });
                            });

                            // Tabla de productos
                            column.Item().PaddingTop(20).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(25); // N°
                                    columns.RelativeColumn(3);   // Producto
                                    columns.RelativeColumn();    // Cantidad
                                    columns.RelativeColumn();    // Precio Unitario
                                    columns.RelativeColumn();    // Total
                                });

                                // Encabezados
                                table.Header(header =>
                                {
                                    header.Cell().Background(Colors.Blue.Lighten5).Text("#").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten5).Text("Producto").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten5).AlignRight().Text("Cantidad").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten5).AlignRight().Text("P. Unitario").Bold();
                                    header.Cell().Background(Colors.Blue.Lighten5).AlignRight().Text("Total").Bold();
                                });

                                // Productos
                                for (var i = 0; i < factura.Detalles.Count; i++)
                                {
                                    var item = factura.Detalles[i];
                                    var backgroundColor = i % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;

                                    table.Cell().Background(backgroundColor).Text((i + 1).ToString());
                                    table.Cell().Background(backgroundColor).Text(item.Producto.Nombre);
                                    table.Cell().Background(backgroundColor).AlignRight().Text(item.Cantidad.ToString());
                                    table.Cell().Background(backgroundColor).AlignRight().Text(item.PrecioUnitario.ToString("C"));
                                    table.Cell().Background(backgroundColor).AlignRight().Text((item.Cantidad * item.PrecioUnitario).ToString("C"));
                                }

                                // Total
                                table.Cell().ColumnSpan(4).AlignRight().Text("Total:").Bold().FontSize(12);
                                table.Cell().AlignRight().Text(factura.Total.ToString("C")).Bold().FontSize(12);
                            });

                            // Métodos de pago y notas
                            column.Item().PaddingTop(20).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Métodos de Pago:").Bold();
                                    col.Item().Text("- Transferencia Bancaria");
                                    col.Item().Text("- Tarjeta de Crédito");
                                    col.Item().Text("- PayPal");
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Notas:").Bold();
                                    col.Item().Text("Gracias por su compra. Por favor, realice el pago antes de la fecha de vencimiento.");
                                    col.Item().Text("Factura válida como documento tributario.");
                                });
                            });
                        });

                    page.Footer()
                        .BorderTop(1)
                        .PaddingTop(10)
                        .Row(row =>
                        {
                            row.RelativeItem().Text("Facturando Company S.A.");
                            row.RelativeItem().AlignCenter().Text("www.facturando.com");
                            row.RelativeItem().AlignRight().Text(text =>
                            {
                                text.Span("Página ");
                                text.CurrentPageNumber();
                            });
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}