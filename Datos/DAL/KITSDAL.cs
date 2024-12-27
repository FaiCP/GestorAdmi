using Comun.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Modelo.Modelos;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;

namespace Datos.DAL
{
    public partial class KITSDAL
    {
        public static ListadoPaginadoVMR<KitsVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<KitsVMR> resultado = new ListadoPaginadoVMR<KitsVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.Kits.Select(x => new KitsVMR
                {
                    id = x.id,
                    ESTADO = x.ESTADO,
                    INSUMO = x.INSUMO,
                    CANTIDAD = x.CANTIDAD,
                    MARCA = x.MARCA,
                    OBSERVACION = x.OBSERVACION,
                    MODELO = x.MODELO,
                    Serie = x.Serie,                    
                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                            || x.MARCA.Contains(textoBusqueda)
                                            || x.INSUMO.Contains(textoBusqueda)
                                            || x.Serie.Contains(textoBusqueda)
                                            || x.MODELO.Contains(textoBusqueda)

                    );
                }

                resultado.cantidadTotal = query.Count();

                resultado.elementos = query
                    .OrderBy(x => x.id)
                    .Skip(pagina * cantidad)
                    .Take(cantidad)
                    .ToList();

            }

            return resultado;
        }

        public static long Crear(KitsVMR nuevoItem)
        {
            using (var db = DbConexion.Create())
            {
                var KITS = new Kits
                {
                    id= nuevoItem.id,
                    Serie = nuevoItem.Serie,
                    OBSERVACION = nuevoItem.OBSERVACION,
                    CANTIDAD = nuevoItem.CANTIDAD,
                    ESTADO = nuevoItem.Serie,
                    INSUMO = nuevoItem.INSUMO,
                    MARCA = nuevoItem.MARCA,
                    MODELO = nuevoItem.MODELO
                };
                db.Kits.Add(KITS);
                db.SaveChanges();
            }

            return nuevoItem.id;

        }
        public static void Actualizar(KitsVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.Kits.Find(item.id);
                itemUpdate.Serie = item.Serie;
                itemUpdate.OBSERVACION = item.OBSERVACION;
                itemUpdate.INSUMO = item.INSUMO;
                itemUpdate.CANTIDAD = item.CANTIDAD;
                itemUpdate.MODELO = item.MODELO;
                itemUpdate.MARCA = item.MARCA;
                itemUpdate.MODELO = item.MODELO;
                db.Kits.Add(itemUpdate);

                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.Kits.Where(x => ids.Contains(x.id));

                foreach (var item in itemsDelete)
                {
                    
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        public static List<KitsVMR> ObtenerKits()
        {
            using (var db = DbConexion.Create())
            {
                var equipos = (from gh in db.Kits                               
                               select new KitsVMR
                               {
                                   id = gh.id,
                                   CANTIDAD = gh.CANTIDAD,
                                   ESTADO = gh.ESTADO,
                                   INSUMO = gh.INSUMO,
                                   MARCA = gh.MARCA,
                                   MODELO = gh.MODELO,
                                   OBSERVACION = gh.OBSERVACION,
                                   Serie = gh.Serie,
                                   

                               }).ToList();

                return equipos;
            }
        }

        public static byte[] GenerarActaPDF()
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var equiposInfo = ObtenerKits();
                    Document document = new Document(PageSize.A4, 36, 36, 100, 100);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    writer.PageEvent = new EncabezadosVMR();
                    document.Open();

                    var equipoSeleccionado = equiposInfo.FirstOrDefault();

                    if (equipoSeleccionado == null)
                    {
                        throw new Exception("No se encontró ningún equipo para generar el acta.");
                    }

                    // Título del Acta

                    Paragraph tituloActa = new Paragraph("Informe de Inventarios\n\n", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };

                    document.Add(tituloActa);


                    // Crear una fuente con tamaño 10
                    Font font = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL);

                    // Tabla de Detalle del Equipo
                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 2, 1.4f });

                    // Encabezados (aplicar fuente a los encabezados)
                    table.AddCell(new PdfPCell(new Phrase("Nº", font)));
                    table.AddCell(new PdfPCell(new Phrase("INSUMO", font)));
                    table.AddCell(new PdfPCell(new Phrase("MARCA", font)));
                    table.AddCell(new PdfPCell(new Phrase("MODELO", font)));
                    table.AddCell(new PdfPCell(new Phrase("Serie", font)));
                    table.AddCell(new PdfPCell(new Phrase("CANTIDAD", font)));
                    table.AddCell(new PdfPCell(new Phrase("ESTADO", font)));
                    table.AddCell(new PdfPCell(new Phrase("OBSERVACION", font)));

                    // Agregar los datos de los equipos (también con la misma fuente)
                    int contador = 1;
                    foreach (var equipo in equiposInfo)
                    {
                        table.AddCell(new PdfPCell(new Phrase(contador.ToString(), font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.INSUMO, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.MARCA, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.MODELO, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Serie, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.CANTIDAD, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.ESTADO, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.OBSERVACION, font)));
                        contador++;
                    }

                    document.Add(table);


                    document.Close();

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el acta en PDF", ex);
            }
        }
    }
}
