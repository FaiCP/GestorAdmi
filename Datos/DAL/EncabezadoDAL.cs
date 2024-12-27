using Comun.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    public partial class EncabezadoDAL : PdfPageEventHelper
    {
        private List<ActasMVR> equiposInfo;

        public EncabezadoDAL(List<ActasMVR> equipos)
        {
            this.equiposInfo = equipos;
        }
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);

            // Crear tabla para el encabezado
            PdfPTable headerTable = new PdfPTable(1);
            headerTable.WidthPercentage = 100;
            headerTable.SetWidths(new float[] { 1});

            string logoUrl = "https://i.postimg.cc/76n2VdB1/Captura1.png";
            using (var httpClient = new HttpClient())
            {
                var logoBytes = httpClient.GetByteArrayAsync(logoUrl).Result;
                var logo = Image.GetInstance(logoBytes);
                logo.ScaleToFit(80, 80); // Ajustar tamaño
                var logoCell = new PdfPCell(logo)
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                headerTable.AddCell(logoCell);
            }

            // Agregar el encabezado al documento
            headerTable.WriteSelectedRows(0, -1, 0, document.Top, writer.DirectContent);
        }

        // Definir el pie de página (Footer)
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            // Crear tabla para el pie de página
            PdfPTable footerTable = new PdfPTable(2);
            footerTable.WidthPercentage = 100;
            footerTable.SetWidths(new float[] { 1, 1 });

            var leftTextCell = new PdfPCell
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_MIDDLE
            };
            leftTextCell.AddElement(new Paragraph("¡Compromiso con la Democracia!", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
            footerTable.AddCell(leftTextCell);

            string footerImagePath = "https://i.postimg.cc/76yj8HJ7/Captura.png";
            using (var httpClient = new HttpClient())
            {
                var logoBytes = httpClient.GetByteArrayAsync(footerImagePath).Result;
                var footerImage = Image.GetInstance(logoBytes);
                footerImage.ScaleToFit(150, 150);
                var footerImgCell = new PdfPCell(footerImage)
                {
                    Border = PdfPCell.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                footerTable.AddCell(footerImgCell);
            }

            // Agregar el pie de página al documento
            footerTable.WriteSelectedRows(0, -1, 0, document.Bottom - 10, writer.DirectContent);
        }
    }
}
