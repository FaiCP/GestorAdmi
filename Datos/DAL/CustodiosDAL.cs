using Comun.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Modelo.Modelo;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static iTextSharp.text.pdf.AcroFields;

namespace Datos.DAL
{
    public partial class CustodiosDAL
    {
        
        public static ListadoPaginadoVMR<CustodioVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<CustodioVMR> resultado = new ListadoPaginadoVMR<CustodioVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.Custodios.Select(x => new CustodioVMR
                {
                    id = x.id,
                    nombre_empleado = x.nombre,
                    cargo_empleado = x.cargo,
                    cedula_empleado = x.cedula,
                    departamento = (from d in db.departamentos
                                       where d.id == x.id_departamento
                                       select d.nombre).FirstOrDefault(),
                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.nombre_empleado.Contains(textoBusqueda)
                                            || x.cedula_empleado.Contains(textoBusqueda)

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

        public static long Crear(Custodios nuevoItem)
        {
            using (var db = DbConexion.Create())
            {
                var custodios = new Custodios
                {
                    cargo = nuevoItem.cargo,
                    cedula = nuevoItem.cedula,
                    id_departamento = nuevoItem.id_departamento,
                    nombre = nuevoItem.nombre,

                };
                db.Custodios.Add(custodios);
                db.SaveChanges();
            }

            return nuevoItem.id;

        }

        public static List<ActasMVR> ObtenerReporteGeneral()
        {
            using (var db = DbConexion.Create())
            {
                var equipos = (from ga in db.gestion_activos
                               where !(bool)ga.borrado
                               select new ActasMVR
                               {
                                   Id = ga.id,
                                   id_equipo = ga.id_equipo,
                                   Fecha = (DateTime)ga.fecha_asignacion,
                                   Descripcion = ga.gestion_hardware.observacion,
                                   nombre_dispositivo = ga.gestion_hardware.nombre_dispositivo,
                                   Marca = ga.gestion_hardware.marca,
                                   Modelo = ga.gestion_hardware.modelo,
                                   CodigoCNE = ga.gestion_hardware.codigo_cne,
                                   Estado = ga.gestion_hardware.estado,
                                   NombreCustodio1 = (from hc in db.Custodios
                                                      where hc.id == ga.id_custodio
                                                      select hc.nombre).FirstOrDefault(),
                                   valor = ga.gestion_hardware.valor
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

                    var equiposInfo = ObtenerReporteGeneral();
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

                    Paragraph tituloActa = new Paragraph("REPORTE GENERAL DE INVENTARIO\n\n", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };

                    document.Add(tituloActa);

                    // Tabla de Detalle del Equipo
                    PdfPTable table = new PdfPTable(10);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 1, 2, 1, 2, 2, 2, 2, 2, 2, 1 });

                    // Encabezados
                    table.AddCell("Nº");
                    table.AddCell("CUSTODIO");
                    table.AddCell("MARCA");
                    table.AddCell("MODELO");
                    table.AddCell("N° SERIE");
                    table.AddCell("COD CNE");
                    table.AddCell("FECHA INGRESO");
                    table.AddCell("VALOR BIEN");
                    table.AddCell("ESTADO");
                    table.AddCell("EQUIPO");

                    // Agregar los datos de los equipos
                    int contador = 1;
                    foreach (var equipo in equiposInfo)
                    {
                        table.AddCell(contador.ToString());
                        table.AddCell(equipo.NombreCustodio1);
                        table.AddCell(equipo.Marca);
                        table.AddCell(equipo.Modelo);
                        table.AddCell(equipo.CodigoCNE);
                        table.AddCell(equipo.id_equipo);
                        table.AddCell(equipo.Fecha.ToString("dd/MM/yyyy"));
                        table.AddCell(equipo.valor.ToString());
                        table.AddCell(equipo.Estado);
                        table.AddCell(equipo.nombre_dispositivo);
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
