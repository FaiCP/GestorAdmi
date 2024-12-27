using System;
using Comun.ViewModels;
using Modelo.Modelos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;
using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Data.Entity;
using static System.Net.WebRequestMethods;
using System.Net.Http;
using iTextSharp.text.pdf.codec.wmf;

namespace Datos.DAL
{
    public partial class GestionActuvosDAL
    {
        public static ListadoPaginadoVMR<GestionActivosVMR> LeerTodo(int cantidad, int pagina, string busqueda)
        {
            ListadoPaginadoVMR<GestionActivosVMR> resultado = new ListadoPaginadoVMR<GestionActivosVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.gestion_activos.Where(x => (bool)!x.borrado).Select(x => new GestionActivosVMR
                {
                    id = x.id,
                    id_equipo = x.id_equipo,
                    fecha_asignacion = x.fecha_asignacion,
                    fecha_devolucion = x.fecha_devolucion,
                    borrado = x.borrado,
                    nombre_empleado = db.Custodios.Where(c => c.id == x.id_custodio).Select(c => c.nombre)
                .FirstOrDefault(),
                    marca = db.gestion_hardware.Where(gh=> gh.id_equipo == x.id_equipo).Select(gh=>gh.marca)
                .FirstOrDefault(),
                    estado = db.gestion_hardware.Where(gh => gh.id_equipo == x.id_equipo).Select(gh => gh.estado)
                .FirstOrDefault(),
                    modelo = db.gestion_hardware.Where(gh => gh.id_equipo == x.id_equipo).Select(gh => gh.modelo)
                .FirstOrDefault(),
                    codigo_cne = db.gestion_hardware.Where(gh => gh.id_equipo == x.id_equipo).Select(gh => gh.codigo_cne)
                .FirstOrDefault(),
                    nombre_dispositivo = db.gestion_hardware.Where(gh => gh.id_equipo == x.id_equipo).Select(gh => gh.nombre_dispositivo)
                .FirstOrDefault(),
                });

                if (!string.IsNullOrEmpty(busqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(busqueda)
                                            || x.marca.Contains(busqueda)
                                            || x.nombre_dispositivo.Contains(busqueda)
                                            || x.codigo_cne.Contains(busqueda)
                                            || x.nombre_empleado.Contains(busqueda)

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

        public static List<long?> Crear(IEnumerable<gestion_activos> items)
        {
            var ids = new List<long?>();

            using (var db = DbConexion.Create())
            {
                foreach (var item in items)
                {
                    var equipoExistente = db.gestion_activos
                        .FirstOrDefault(e => e.id_equipo == item.id_equipo && e.fecha_devolucion == null && (bool)!e.borrado);

                    if (equipoExistente != null)
                    {
                        throw new InvalidOperationException($"El equipo con ID {item.id_equipo} ya está asignado y no tiene fecha de devolución.");
                    }

                    item.borrado = false;
                    item.fecha_asignacion = DateTime.Now;

                    db.gestion_activos.Add(item);
                    db.SaveChanges();

                    ids.Add(item.id);
                }
            }

            return ids;
        }


        public static void Actualizar(GestionActivosVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.gestion_activos.Find(item.id);

                itemUpdate.fecha_devolucion = item.fecha_devolucion;
                itemUpdate.borrado = true;
                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.gestion_activos.Where(x => ids.Contains(x.id));

                foreach (var item in itemsDelete)
                {
                    item.borrado = true;
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
        public static List<ActasMVR> ObtenerEquiposConCustodio(List<long> id_activo)
        {
            using (var db = DbConexion.Create())
            {
                var equipos = (from ga in db.gestion_activos
                               where !(bool)ga.borrado && id_activo.Contains(ga.id)
                               select new ActasMVR
                               {
                                   Id = ga.id,
                                   id_equipo= ga.id_equipo,
                                   Fecha = DateTime.Now,
                                   Descripcion= ga.gestion_hardware.observacion,
                                   nombre_dispositivo = ga.gestion_hardware.nombre_dispositivo,
                                   Marca = ga.gestion_hardware.marca,
                                   Modelo = ga.gestion_hardware.modelo,                                   
                                   CodigoCNE = ga.gestion_hardware.codigo_cne,
                                   Estado = ga.gestion_hardware.estado,
                                   NombreCustodio1 = (from hc in db.Custodios
                                                     where hc.id == ga.id_custodio
                                                     select hc.nombre).FirstOrDefault(),
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
        public static List<ActasMVR> ObtenerEquiposConCustodioDevolucion(List<long> id_activo)
        {
            using (var db = DbConexion.Create())
            {
                var equipos = (from ga in db.gestion_activos
                               where (bool)ga.borrado && id_activo.Contains(ga.id)
                               select new ActasMVR
                               {
                                   Id = ga.id,
                                   id_equipo = ga.id_equipo,
                                   Fecha = (DateTime)ga.fecha_devolucion,
                                   Descripcion = ga.gestion_hardware.observacion,
                                   nombre_dispositivo = ga.gestion_hardware.nombre_dispositivo,
                                   Marca = ga.gestion_hardware.marca,
                                   Modelo = ga.gestion_hardware.modelo,
                                   CodigoCNE = ga.gestion_hardware.codigo_cne,
                                   Estado = ga.gestion_hardware.estado,
                                   NombreCustodio1 = (from hc in db.Custodios
                                                      where hc.id == ga.id_custodio
                                                      select hc.nombre).FirstOrDefault(),
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
                    var equiposInfo = ObtenerEquiposConCustodio(id_activo);

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

                    Paragraph tituloActa = new Paragraph("ACTA ENTREGA-RECEPCIÓN\n\n", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };

                    document.Add(tituloActa);
                    document.Add(new Paragraph($"En la ciudad de Puyo, a los {equipoSeleccionado.Fecha:dd} días del mes de {equipoSeleccionado.Fecha:MMMM} del año {equipoSeleccionado.Fecha:yyyy}, en las instalaciones de la Delegación Provincial Electoral del Consejo Nacional Electoral - Pastaza, comparecen por una parte el ING.NELSON RICARDO CARDENAS HERMOSA - TECNICO ELECTORAL 2- y por otra parte EL ING. {equipoSeleccionado.NombreCustodio1}- {equipoSeleccionado.cargo1}, quienes proceden con la entrega recepción de equipos informáticos propiedad de la Delegación Provincial Electoral de Pastaza, conforme al siguiente detalle:\n\n", new Font(Font.FontFamily.HELVETICA, 12)));

                    Font font = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
                    // Tabla de Detalle del Equipo
                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 0.5f, 1.6f, 1.5f, 2, 2, 1.5f, 1.4f });

                    // Encabezados
                    table.AddCell(new PdfPCell(new Phrase("Nº",font)));
                    table.AddCell(new PdfPCell(new Phrase("DESCRIPCION", font)));
                    table.AddCell(new PdfPCell(new Phrase("MARCA", font)));
                    table.AddCell(new PdfPCell(new Phrase("MODELO", font)));
                    table.AddCell(new PdfPCell(new Phrase("SERIE", font)));
                    table.AddCell(new PdfPCell(new Phrase("ACTIVO", font)));
                    table.AddCell(new PdfPCell(new Phrase("ESTADO", font)));

                    // Agregar los datos de los equipos
                    int contador = 1;
                    foreach (var equipo in equiposInfo)
                    {
                        table.AddCell(new PdfPCell(new Phrase(contador.ToString(),font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.nombre_dispositivo,font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Marca, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Modelo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.CodigoCNE, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.id_equipo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Estado, font)));
                        contador++;
                    }

                    document.Add(table);

                    // Espacio reservado para las firmas
                    PdfPTable firmasTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    firmasTable.SetWidths(new float[] { 2, 2 });

                    // Verificar si hay suficiente espacio antes de agregar firmas
                    if (document.Top - firmasTable.TotalHeight < 150)
                    {
                        document.NewPage();
                    }

                    // Sección de firmas
                    var Custodio1 = new PdfPCell
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    Custodio1.AddElement(new Paragraph("\nENTREGO CONFORME:\n\n", new Font(Font.FontFamily.HELVETICA, 12)));                    
                    Custodio1.AddElement(new Paragraph($"\n\n\n\nING. NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL 2\r\n\r\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    firmasTable.AddCell(Custodio1);

                    var Custodio2 = new PdfPCell
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };

                    Custodio2.AddElement(new Paragraph("\nRECIBO CONFORME:\n\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    Custodio2.AddElement(new Paragraph($"\n\n\n\n{equipoSeleccionado.NombreCustodio1} - {equipoSeleccionado.cargo1}\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    firmasTable.AddCell(Custodio2);

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

        public static byte[] GenerarDevolucionPDF(List<long> id_activo)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var equiposInfo = ObtenerEquiposConCustodioDevolucion(id_activo);

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

                    Paragraph tituloActa = new Paragraph("ACTA ENTREGA-RECEPCIÓN \n\n", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };

                    document.Add(tituloActa);
                    document.Add(new Paragraph($"En la ciudad de Puyo, a los {equipoSeleccionado.Fecha:dd} días del mes de {equipoSeleccionado.Fecha:MMMM} del año {equipoSeleccionado.Fecha:yyyy}, en las instalaciones de la Delegación Provincial Electoral del Consejo Nacional Electoral - Pastaza, comparecen por una parte el ING.NELSON RICARDO CARDENAS HERMOSA - TECNICO ELECTORAL 2  y por otra parte EL ING. {equipoSeleccionado.NombreCustodio1}- RESPONSABLE DE {equipoSeleccionado.cargo1}, quienes proceden con la entrega recepción de equipos informáticos propiedad de la Delegación Provincial Electoral de Pastaza, conforme al siguiente detalle:\n\n", new Font(Font.FontFamily.HELVETICA, 12)));

                    Font font = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL);
                    // Tabla de Detalle del Equipo
                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 1.4f });

                    // Encabezados
                    table.AddCell(new PdfPCell(new Phrase("Nº", font)));
                    table.AddCell(new PdfPCell(new Phrase("DESCRIPCION", font)));
                    table.AddCell(new PdfPCell(new Phrase("MARCA", font)));
                    table.AddCell(new PdfPCell(new Phrase("MODELO", font)));
                    table.AddCell(new PdfPCell(new Phrase("SERIE", font)));
                    table.AddCell(new PdfPCell(new Phrase("ACTIVO", font)));
                    table.AddCell(new PdfPCell(new Phrase("ESTADO", font)));

                    // Agregar los datos de los equipos
                    int contador = 1;
                    foreach (var equipo in equiposInfo)
                    {
                        table.AddCell(new PdfPCell(new Phrase(contador.ToString(), font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.nombre_dispositivo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Marca, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Modelo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.CodigoCNE, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.id_equipo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Estado, font)));
                        contador++;
                    }

                    document.Add(table);

                    // Espacio reservado para las firmas
                    PdfPTable firmasTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    firmasTable.SetWidths(new float[] { 2, 2 });

                    // Verificar si hay suficiente espacio antes de agregar firmas
                    if (document.Top - firmasTable.TotalHeight < 150)
                    {
                        document.NewPage();
                    }

                    // Sección de firmas
                    var Custodio1 = new PdfPCell
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    Custodio1.AddElement(new Paragraph("\nENTREGO CONFORME:\n\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    Custodio1.AddElement(new Paragraph($"\n\n\n\n{equipoSeleccionado.NombreCustodio1} - {equipoSeleccionado.cargo1}\r\n\r\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    firmasTable.AddCell(Custodio1);

                    var Custodio2 = new PdfPCell
                    {
                        Border = PdfPCell.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };

                    Custodio2.AddElement(new Paragraph("\nRECIBO CONFORME:\n\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    Custodio2.AddElement(new Paragraph($"\n\n\n\nING. NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL 2\n", new Font(Font.FontFamily.HELVETICA, 12)));
                    firmasTable.AddCell(Custodio2);

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
