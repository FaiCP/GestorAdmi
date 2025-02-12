using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Net;

namespace Comun.ViewModels
{
    public partial class EncabezadoPersonalVMR : PdfPageEventHelper
    {
        private const string LogoUrl = "https://i.postimg.cc/Cx6k1v1n/logo.png";

        public EncabezadoPersonalVMR() { }

        // Método para cargar imagen de forma síncrona desde una URL
        private Image LoadImage(string imageUrl)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var imageBytes = client.DownloadData(imageUrl);
                    return Image.GetInstance(imageBytes);
                }
            }
            catch (Exception ex)
            {
                // Manejo de error: devuelve un marcador en lugar de interrumpir la generación del PDF
                Console.WriteLine($"Error al descargar la imagen desde {imageUrl}: {ex.Message}");
                // Devuelve un espacio vacío para que el diseño se mantenga
                var placeholder = Image.GetInstance(new byte[0]);
                placeholder.ScaleToFit(60, 60); // Ajusta el tamaño del espacio vacío
                return placeholder;
            }
        }


        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            // Crear la tabla con 3 columnas
            PdfPTable headerTable = new PdfPTable(3)
            {
                WidthPercentage = 100 // Ajustar al 100% del ancho de la página
            };
            headerTable.SetWidths(new float[] { 1, 3, 1 }); // Ajustar proporciones: logo, texto principal, numeración

            // Agregar logo a la celda izquierda
            var logo = LoadImage(LogoUrl); // Usa tu método LoadImage
            logo.ScaleToFit(60, 60); // Ajusta el tamaño del logo
            var logoCell = new PdfPCell(logo)
            {
                
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                Border = PdfPCell.NO_BORDER
            };
            headerTable.AddCell(logoCell);

            // Agregar texto al centro
            var titleCell = new PdfPCell(new Phrase("Acta Entrega-Recepción de Credenciales", new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            headerTable.AddCell(titleCell);

            // Agregar número de página a la celda derecha
            var pageCell = new PdfPCell(new Phrase("Pág.: 1 de 1", new Font(Font.FontFamily.HELVETICA, 10)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            headerTable.AddCell(pageCell);

            // Ajustar el ancho total de la tabla para que ocupe el espacio completo de la página
            headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

            // Escribir la tabla en la página (ajustar la posición)
            float yPosition = document.PageSize.Height - 10;  // Ajusta la posición para que no se superponga
            headerTable.WriteSelectedRows(0, -1, document.LeftMargin, yPosition, writer.DirectContent);

        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            Font boldFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
            // Crear tabla para el pie de página
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.WidthPercentage = 100;
            footerTable.SetWidths(new float[] { 1 });


            PdfPCell footerTextCell = new PdfPCell(new Phrase("Código: F0-08(PG-CG-AD-01); versión: 4", boldFont))
            {
                Border = PdfPCell.NO_BORDER, // Sin bordes
                HorizontalAlignment = Element.ALIGN_LEFT, // Centrado
                VerticalAlignment = Element.ALIGN_MIDDLE // Centrado vertical
            };
            footerTable.AddCell(footerTextCell);

            // Ajustar el ancho total de la tabla para que ocupe el espacio completo de la página
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

            // Escribir el pie de página en la página (ajustar la posición)
            float yPosition = document.Bottom - 10;  // Ajustar la posición para que la imagen no quede demasiado abajo
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, yPosition, writer.DirectContent);
        }
    }
}
