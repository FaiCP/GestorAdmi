using System;
using Comun.ViewModels;
using Modelo.Modelos;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Modelo.Modelo;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Data.Entity;

namespace Datos.DAL
{

    public partial class HardwareDAL
    {
        public static ListadoPaginadoVMR<HaedwareVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<HaedwareVMR> resultado = new ListadoPaginadoVMR<HaedwareVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.gestion_hardware.Where(x => (bool)!x.borrado).Select(x => new HaedwareVMR
                {
                    id = x.id,
                    id_equipo = x.id_equipo,
                    descripcion = x.observacion,
                    nombre_dispositivo = x.nombre_dispositivo,
                    marca = x.marca,
                    modelo = x.modelo,
                    fecha_adquisicion = x.fecha_adquisicion,
                    estado = x.estado,
                    ubicacion = x.ubicacion,
                    codigo_cne = x.codigo_cne,
                    valor = x.valor,
                    NombreCustodio1 = (from ga in db.gestion_activos
                                       join c in db.Custodios on ga.id_custodio equals c.id
                                       where ga.id_equipo == x.id_equipo && (bool)!ga.borrado
                                       select c.nombre).FirstOrDefault(),

                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                            || x.marca.Contains(textoBusqueda)
                                            || x.nombre_dispositivo.Contains(textoBusqueda)
                                            || x.codigo_cne.Contains(textoBusqueda)
                                            || x.modelo.Contains(textoBusqueda)
                                            || x.NombreCustodio1.Contains(textoBusqueda)

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
        public static long Crear(HardwareCaracterVMR nuevoItem)
        {
            using (var db = DbConexion.Create())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var existingItem = db.gestion_hardware.FirstOrDefault(h => h.id_equipo == nuevoItem.id_equipo);
                        if (existingItem != null)
                        {
                            throw new Exception($"Ya existe un equipo con el id {nuevoItem.id_equipo}. Elija otro ID.");
                        }
                        var gestionHardware = new gestion_hardware
                        {
                            id_equipo = nuevoItem.id_equipo,
                            observacion = nuevoItem.descripcion,
                            marca = nuevoItem.marca,
                            modelo = nuevoItem.modelo,
                            estado = nuevoItem.estado,
                            ubicacion = nuevoItem.ubicacion,
                            codigo_cne = nuevoItem.codigo_cne,
                            nombre_dispositivo = nuevoItem.nombre_dispositivo,
                            valor = nuevoItem.valor,
                            borrado = false,
                            fecha_adquisicion = DateTime.Now
                        };

                        db.gestion_hardware.Add(gestionHardware);

                        if (nuevoItem.nombre_dispositivo.ToLower() == "laptop" ||
                            nuevoItem.nombre_dispositivo.ToLower() == "computadora portatil" ||
                            nuevoItem.nombre_dispositivo.ToLower() == "computadora" ||
                            nuevoItem.nombre_dispositivo.ToLower() == "cpu")
                        {
                            var caracteristicas = new caracteristicas_computadora
                            {
                                id_equipo = gestionHardware.id_equipo,
                                ram = nuevoItem.ram,
                                rom = nuevoItem.rom,
                                procesador = nuevoItem.procesador
                            };

                            db.caracteristicas_computadora.Add(caracteristicas);                            
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        return gestionHardware.id;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error al crear hardware", ex);
                    }
                }
            }
        }       

        public static void Actualizar(HaedwareVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.gestion_hardware.Find(item.id_equipo);
                itemUpdate.ubicacion = item.ubicacion;
                itemUpdate.observacion = item.descripcion;
                itemUpdate.estado = item.estado;
                itemUpdate.marca = item.marca;
                itemUpdate.modelo = item.modelo;
                itemUpdate.observacion = item.descripcion;
                itemUpdate.nombre_dispositivo = item.nombre_dispositivo;

                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.gestion_hardware.Where(x => ids.Contains(x.id));

                foreach (var item in itemsDelete)
                {
                    item.borrado = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        public static List<HaedwareVMR> Obtenerhardwares()
        {
            using (var db = DbConexion.Create())
            {
                var equipos = (from gh in db.gestion_hardware
                               where !(bool)gh.borrado
                               select new HaedwareVMR
                               {
                                   id = gh.id,
                                   id_equipo = gh.id_equipo,
                                   descripcion = gh.observacion,
                                   nombre_dispositivo = gh.nombre_dispositivo,
                                   marca = gh.marca,
                                   modelo = gh.modelo,
                                   codigo_cne = gh.codigo_cne,
                                   estado = gh.estado,
                                   valor = gh.valor,
                                   ram = (from ca in db.caracteristicas_computadora
                                          where ca.id_equipo == gh.id_equipo
                                          select ca.ram).FirstOrDefault(),
                                   rom = (from ca in db.caracteristicas_computadora
                                          where ca.id_equipo == gh.id_equipo
                                          select ca.rom).FirstOrDefault(),
                                   Procesador = (from ca in db.caracteristicas_computadora
                                          where ca.id_equipo == gh.id_equipo
                                          select ca.procesador).FirstOrDefault(),
                                   NombreCustodio1 = (from ga in db.gestion_activos
                                                      join c in db.Custodios on ga.id_custodio equals c.id
                                                      where ga.id_equipo == gh.id_equipo
                                                      select c.nombre).FirstOrDefault(),

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
                    var equiposInfo = Obtenerhardwares();
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
                    PdfPTable table = new PdfPTable(12);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 0.5f, 1.4f, 2, 2, 1.5f, 2, 2,1 ,1 ,1 , 1, 1.4f });

                    // Encabezados (aplicar fuente a los encabezados)
                    table.AddCell(new PdfPCell(new Phrase("Nº", font)));
                    table.AddCell(new PdfPCell(new Phrase("ACTIVO", font)));
                    table.AddCell(new PdfPCell(new Phrase("CUSTODIO", font)));
                    table.AddCell(new PdfPCell(new Phrase("DESCRIPCION", font)));
                    table.AddCell(new PdfPCell(new Phrase("MARCA", font)));
                    table.AddCell(new PdfPCell(new Phrase("MODELO", font)));
                    table.AddCell(new PdfPCell(new Phrase("SERIE", font)));
                    table.AddCell(new PdfPCell(new Phrase("RAM", font)));
                    table.AddCell(new PdfPCell(new Phrase("DISCO DURO", font)));
                    table.AddCell(new PdfPCell(new Phrase("PROCESADOR", font)));
                    table.AddCell(new PdfPCell(new Phrase("VALOR", font)));
                    table.AddCell(new PdfPCell(new Phrase("ESTADO", font)));

                    // Agregar los datos de los equipos (también con la misma fuente)
                    int contador = 1;
                    foreach (var equipo in equiposInfo)
                    {
                        table.AddCell(new PdfPCell(new Phrase(contador.ToString(), font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.id_equipo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.NombreCustodio1, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.nombre_dispositivo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.marca, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.modelo, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.codigo_cne, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.ram, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.rom, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.Procesador, font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.valor.ToString(), font)));
                        table.AddCell(new PdfPCell(new Phrase(equipo.estado, font)));
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


        public static byte[] GenerarActaExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using (var stream = new MemoryStream())
                {
                    var equiposInfo = Obtenerhardwares();

                    if (equiposInfo == null || !equiposInfo.Any())
                    {
                        throw new Exception("No se encontró ningún equipo para generar el acta.");
                    }

                    // Crear un nuevo paquete de Excel
                    using (var package = new ExcelPackage(stream))
                    {
                        // Agregar una nueva hoja de trabajo
                        var worksheet = package.Workbook.Worksheets.Add("Informe de Inventarios");

                        // Título del Acta
                        worksheet.Cells[1, 1].Value = "Informe de Inventarios";
                        worksheet.Cells[1, 1].Style.Font.Size = 16;
                        worksheet.Cells[1, 1].Style.Font.Bold = true;
                        worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[1, 1, 1, 12].Merge = true; // Merge title across columns

                        // Encabezados de la tabla
                        string[] headers = { "Nº", "ACTIVO", "CUSTODIO", "DESCRIPCION", "MARCA", "MODELO", "SERIE", "RAM","DISCO DURO", "PROCESADOR", "VALOR", "ESTADO" };
                        for (int i = 0; i < headers.Length; i++)
                        {
                            worksheet.Cells[3, i + 1].Value = headers[i];
                            worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                            worksheet.Cells[3, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }

                        // Agregar los datos de los equipos
                        int row = 4; // Comenzar en la fila 4
                        int contador = 1;
                        foreach (var equipo in equiposInfo)
                        {
                            worksheet.Cells[row, 1].Value = contador++;
                            worksheet.Cells[row, 2].Value = equipo.id_equipo;
                            worksheet.Cells[row, 3].Value = equipo.NombreCustodio1;
                            worksheet.Cells[row, 4].Value = equipo.nombre_dispositivo;
                            worksheet.Cells[row, 5].Value = equipo.marca;
                            worksheet.Cells[row, 6].Value = equipo.modelo;
                            worksheet.Cells[row, 7].Value = equipo.codigo_cne;
                            worksheet.Cells[row, 7].Value = equipo.ram;
                            worksheet.Cells[row, 7].Value = equipo.rom;
                            worksheet.Cells[row, 7].Value = equipo.Procesador;
                            worksheet.Cells[row, 8].Value = equipo.valor;
                            worksheet.Cells[row, 9].Value = equipo.estado;
                            row++;
                        }

                        // Ajustar el ancho de las columnas
                        for (int i = 1; i <= headers.Length; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        // Guardar el paquete
                        package.Save();
                    }

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el acta en Excel", ex);
            }
        }

    }
}
