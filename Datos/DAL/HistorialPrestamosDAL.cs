using Comun.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    public partial class HistorialPrestamosDAL
    {
        public static ListadoPaginadoVMR<HistorialPrestamosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda, int? idCustodio)
        {
            ListadoPaginadoVMR<HistorialPrestamosVMR> resultado = new ListadoPaginadoVMR<HistorialPrestamosVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.gestion_activos
                    .Where(x => x.id_custodio == idCustodio) 
                    .Select(x => new HistorialPrestamosVMR
                    {
                        id = x.id,
                        id_equipo = x.id_equipo,
                        fecha_asignacion = x.fecha_asignacion,
                        fecha_devolucion = x.fecha_devolucion,
                        nombre_empleado = (from ga in db.gestion_activos
                                           join c in db.Custodios on ga.id_custodio equals c.id
                                           where ga.id_equipo == x.id_equipo
                                           select c.nombre).FirstOrDefault(),
                        nombre_departamento = (from ga in db.gestion_activos
                                               join c in db.Custodios on ga.id_custodio equals c.id
                                               join d in db.departamentos on c.id_departamento equals d.id
                                               where ga.id_equipo == x.id_equipo
                                               select d.nombre).FirstOrDefault(),
                        nombre_dispositivo = (from ga in db.gestion_activos
                                              join h in db.gestion_hardware on ga.id_equipo equals h.id_equipo
                                              where ga.id_equipo == x.id_equipo
                                              select h.nombre_dispositivo).FirstOrDefault(),
                        codigo_cne = (from ga in db.gestion_activos
                                      join h in db.gestion_hardware on ga.id_equipo equals h.id_equipo
                                      where ga.id_equipo == x.id_equipo
                                      select h.codigo_cne).FirstOrDefault(),
                        hardware = (from h in db.gestion_hardware
                                    where h.id_equipo == x.id_equipo
                                    select new HaedwareVMR
                                    {
                                        estado = h.estado,
                                        marca = h.marca,
                                        modelo = h.modelo,
                                        nombre_dispositivo = h.nombre_dispositivo,
                                        valor = h.valor
                                    }).FirstOrDefault(),
                    });

                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                             || x.nombre_dispositivo.Contains(textoBusqueda)
                                             || x.codigo_cne.Contains(textoBusqueda));
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

        public static List<ActasMVR> ObtenerHistorialPorCustodio(List<long> id_activo)
        {
            using (var db = DbConexion.Create())
            {
                var equipos = (from ga in db.gestion_activos
                               where (bool)ga.borrado && id_activo.Contains(ga.id)
                               select new ActasMVR
                               {
                                   Id = ga.id,
                                   id_equipo = ga.id_equipo,
                                   Fecha = DateTime.Now,
                                   Descripcion = ga.gestion_hardware.observacion,
                                   nombre_dispositivo = ga.gestion_hardware.nombre_dispositivo,
                                   Marca = ga.gestion_hardware.marca,
                                   Modelo = ga.gestion_hardware.modelo,
                                   CodigoCNE = ga.gestion_hardware.codigo_cne,
                                   Estado = ga.gestion_hardware.estado,
                                   NombreCustodio1 = (from hc in db.Custodios
                                                      where hc.id == ga.id_custodio
                                                      select hc.nombre).FirstOrDefault(),
                                   valor = ga.gestion_hardware.valor,
                                   cargo1 = (from hc in db.Custodios
                                             where hc.id == ga.id_custodio
                                             select hc.cargo).FirstOrDefault(),
                                   Departamento = (from dep in db.departamentos
                                                   join hc in db.Custodios on dep.id equals hc.id_departamento
                                                   where hc.id == ga.id_custodio
                                                   select dep.nombre).FirstOrDefault()
                               }).ToList();

                return equipos;
            }
        }
        public static byte[] GenerarActaPDF(List<long> id_activo)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var equiposInfo = ObtenerHistorialPorCustodio(id_activo);

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

                    Paragraph tituloActa = new Paragraph("REPORTE DE BIENES POR CUSTODIO\n\n", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };

                    document.Add(tituloActa);
                    document.Add(new Paragraph($"CUSTODIO: {equipoSeleccionado.NombreCustodio1}\n\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    document.Add(new Paragraph($"DEPARTAMENTO: {equipoSeleccionado.Departamento}\n\n", new Font(Font.FontFamily.HELVETICA, 12)));

                    Font font = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
                    // Tabla de Detalle del Equipo
                    PdfPTable table = new PdfPTable(9);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 2, 1, 1.4f });

                    // Encabezados
                    table.AddCell(new PdfPCell(new Phrase("Nº",font)));
                    table.AddCell(new PdfPCell(new Phrase("CÓDIGO ACTUAL",font)));
                    table.AddCell(new PdfPCell(new Phrase("BIEN",font)));
                    table.AddCell(new PdfPCell(new Phrase("SERIE", font)));
                    table.AddCell(new PdfPCell(new Phrase("MODELO", font)));
                    table.AddCell(new PdfPCell(new Phrase("MARCA", font)));
                    table.AddCell(new PdfPCell(new Phrase("VALOR", font)));
                    table.AddCell(new PdfPCell(new Phrase("ESTADO", font)));
                    table.AddCell(new PdfPCell(new Phrase("OBSERVACIONES", font)));

                    // Agregar los datos de los equipos
                    int contador = 1;
                    foreach (var equipo in equiposInfo)
                    {
                        table.AddCell(new PdfPCell(new Phrase(contador.ToString(),font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.id_equipo,font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.nombre_dispositivo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.CodigoCNE, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Modelo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Marca, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.valor.ToString(), font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Estado, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Descripcion, font)));
                        contador++;
                    }
                    document.Add(table);

                    // Espacio reservado para las firmas
                    PdfPTable firmasTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    firmasTable.SetWidths(new float[] { 75, 25 }); // Proporción del ancho de las columnas

                    var delegadosCell = new PdfPCell
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    delegadosCell.AddElement(new Paragraph("\n\n\n\nRICARDO CARDENAS HERMOZA\r\nDelegado de la Dirección Provincial", new Font(Font.FontFamily.HELVETICA, 12)));
                    delegadosCell.AddElement(new Paragraph("\n\n\nHOMERO CABRERA MEZA\r\nDelegado Unidad Provincial Administrativa", new Font(Font.FontFamily.HELVETICA, 12)));
                    delegadosCell.AddElement(new Paragraph("\n\n\nRUTH ELIZABETH RONDAL LLUGAY\r\nDelegada Unidad Provincial Financiera", new Font(Font.FontFamily.HELVETICA, 12)));
                    firmasTable.AddCell(delegadosCell);
                    var custodioCell = new PdfPCell
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };

                    custodioCell.AddElement(new Paragraph($"\n\n\n\n{equipoSeleccionado.NombreCustodio1} - {equipoSeleccionado.cargo1}\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    firmasTable.AddCell(custodioCell);
                    document.Add(firmasTable);
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
