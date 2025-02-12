using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Net;

namespace Comun.ViewModels
{
    public partial class EncabezadosVMR : PdfPageEventHelper
    {
        private const string LogoUrl = "https://i.postimg.cc/76n2VdB1/Captura1.png";
        private const string FooterImageUrl = "https://i.postimg.cc/76yj8HJ7/Captura.png";

        public EncabezadosVMR() { }

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
                // Manejo de error: devuelve una imagen vacía o un texto con la URL
                Console.WriteLine($"Error al descargar la imagen desde {imageUrl}: {ex.Message}");
                var phrase = new Phrase(imageUrl, FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.RED));
                var paragraph = new Paragraph(phrase);
                return Image.GetInstance(new byte[0]); // Devuelve una imagen vacía
            }
        }

        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            // Crear una tabla con 1 columna para el encabezado (logo)
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100;  // Ancho al 100% de la página
            headerTable.SetWidths(new float[] { 1 });  // Usamos solo una columna para centrar el logo

            // Cargar el logo
            var logo = LoadImage(LogoUrl);
            logo.ScaleToFit(document.PageSize.Width / 1, document.PageSize.Height / 10);  // Ajustar tamaño al 33% del ancho de la página
            var logoCell = new PdfPCell(logo)
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,  // Centrado horizontal
                VerticalAlignment = Element.ALIGN_MIDDLE  // Centrado vertical
            };

            // Agregar la celda con el logo
            headerTable.AddCell(logoCell);

            // Ajustar el ancho total de la tabla para que ocupe el espacio completo de la página
            headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

            // Escribir la tabla en la página (ajustar la posición)
            float yPosition = document.PageSize.Height - 10;  // Ajusta la posición para que no se superponga
            headerTable.WriteSelectedRows(0, -1, document.LeftMargin, yPosition, writer.DirectContent);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            // Crear tabla para el pie de página
            PdfPTable footerTable = new PdfPTable(1);
            footerTable.WidthPercentage = 100;
            footerTable.SetWidths(new float[] { 1 });

            // Cargar la imagen del pie de página
            var footerImage = LoadImage(FooterImageUrl);
            footerImage.ScaleToFit(100, 100);  // Ajustar tamaño al 150x150 píxeles
            var footerImgCell = new PdfPCell(footerImage)
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_RIGHT,  // Centrado horizontal
                VerticalAlignment = Element.ALIGN_MIDDLE  // Centrado vertical
            };

            // Agregar la celda con la imagen del pie de página
            footerTable.AddCell(footerImgCell);

            // Ajustar el ancho total de la tabla para que ocupe el espacio completo de la página
            footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

            // Escribir el pie de página en la página (ajustar la posición)
            float yPosition = document.Bottom -10;  // Ajustar la posición para que la imagen no quede demasiado abajo
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, yPosition, writer.DirectContent);
        }
    }
}
