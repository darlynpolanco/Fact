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
            // Configurar QuestPDF
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text("FACTURA")
                        .SemiBold().FontSize(24).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Información de la factura
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Número: {factura.Id}");
                                    col.Item().Text($"Fecha: {factura.Fecha:dd/MM/yyyy}");
                                });

                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Cliente: {factura.Cliente.Nombre}");
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
                                    header.Cell().Text("#");
                                    header.Cell().Text("Producto");
                                    header.Cell().AlignRight().Text("Cantidad");
                                    header.Cell().AlignRight().Text("P. Unitario");
                                    header.Cell().AlignRight().Text("Total");

                                    header.Cell().ColumnSpan(5).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                                });

                                // Productos
                                for (var i = 0; i < factura.Detalles.Count; i++)
                                {
                                    var item = factura.Detalles[i];

                                    table.Cell().Text(i + 1);
                                    table.Cell().Text(item.Producto.Nombre);
                                    table.Cell().AlignRight().Text(item.Cantidad.ToString());
                                    table.Cell().AlignRight().Text(item.PrecioUnitario.ToString("C"));
                                    table.Cell().AlignRight().Text((item.Cantidad * item.PrecioUnitario).ToString("C"));
                                }

                                // Total
                                table.Cell().ColumnSpan(4).AlignRight().Text("Total:");
                                table.Cell().AlignRight().Text(factura.Total.ToString("C"));
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}
