﻿using Comun.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Modelo.Modelo;
using Modelo.Modelos;
using System.Net.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Xml.Linq;

namespace Datos.DAL
{
    public partial class PersonalDAL
    {
        public static ListadoPaginadoVMR<PersonalVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<PersonalVMR> resultado = new ListadoPaginadoVMR<PersonalVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.Personal.Where(x => (bool)!x.borrado).Select(x => new PersonalVMR
                {
                    Id = x.id,
                    nombre = x.nombre,
                    cedula = x.cedula,
                    fecha = x.fecha,
                    cargo = x.cargo,
                    email = x.email,
                    tempPass = x.tempPass,
                });
                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.nombre.Contains(textoBusqueda)
                                            || x.cedula.Contains(textoBusqueda)

                    );
                }

                resultado.cantidadTotal = query.Count();

                resultado.elementos = query
                    .OrderBy(x => x.Id)
                    .Skip(pagina * cantidad)
                    .Take(cantidad)
                    .ToList();
            }

            return resultado;
        }

        public static long Crear(Personal item)
        {

            using (var db = DbConexion.Create())
            {
                item.borrado = false;
                var palabras = item.nombre.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries);
                if (palabras.Length >= 4)
                {
                    var primerNombre = palabras[0]; 
                    var primerApellido = palabras[2]; 

                    item.email = $"{primerNombre.ToLower()}{primerApellido.ToLower()}@cne.gob.ec";
                }
                else

                item.email = item.nombre;
                item.fecha = DateTime.Now;
                item.tempPass = $"Pastaza{item.fecha:yyyy}";
                db.Personal.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        public static void Actualizar(PersonalVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.Personal.Find(item.Id);
                itemUpdate.email = item.email;
                itemUpdate.cedula = item.cedula;
                itemUpdate.nombre = item.nombre;
                itemUpdate.cargo = item.cargo;
                itemUpdate.tempPass = item.tempPass;
                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static List<PersonalVMR> ObtenerPersonal(List<long> id_activo = null)
        {
            using (var db = DbConexion.Create())
            {
                // Consulta base
                var query = from p in db.Personal
                            where !(bool)p.borrado
                            select new PersonalVMR
                            {
                                Id = p.id,
                                fecha = p.fecha,
                                nombre = p.nombre,
                                cedula = p.cedula,
                                cargo = p.cargo,
                                email = p.email,
                                tempPass = p.tempPass
                            };

                // Si se proporciona una lista de IDs, filtra por estos
                if (id_activo != null && id_activo.Any())
                {
                    query = query.Where(p => id_activo.Contains(p.Id));
                }

                return query.ToList();
            }
        }



        public static byte[] GenerarActaPDF(List<long> id_activo)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var equiposInfo = ObtenerPersonal(id_activo);

                    Document document = new Document(PageSize.A4, 36, 36, 70, 50);
                    PdfWriter writer = PdfWriter.GetInstance(document, stream);
                    writer.PageEvent = new EncabezadoPersonalVMR();
                    document.Open();

                    var equipoSeleccionado = equiposInfo.FirstOrDefault();

                    if (equipoSeleccionado == null)
                    {
                        throw new Exception("No se encontró ningún equipo para generar el acta.");
                    }

                    // Título del Acta

                    Paragraph tituloActa = new Paragraph("ACTA ENTREGA RECEPCIÓN DE CREDENCIALES DIGITALES", new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_CENTER
                    };

                    document.Add(tituloActa);

                    Phrase nombre = new Phrase();
                    nombre.Add(new Chunk("La presenté acta de entrega recepción tiene  por  objeto  otorgar credenciales para el manejo del correo Institucional y Quipux a:", new Font(Font.FontFamily.HELVETICA, 12)));
                    nombre.Add(new Chunk(equipoSeleccionado.nombre, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    nombre.Add(new Chunk(" con nùmero de cedula Nº", new Font(Font.FontFamily.HELVETICA, 12)));
                    nombre.Add(new Chunk (equipoSeleccionado.cedula, new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    nombre.Add(new Chunk($", cuyo cargo es {equipoSeleccionado.cargo} para el proceso Electoral {equipoSeleccionado.fecha:yyyy} El funcionario receptor de las credenciales está obligado al cumplimiento de:\n", new Font(Font.FontFamily.HELVETICA, 12)));

                    Paragraph parrafo = new Paragraph(nombre)
                    {
                        Alignment = Element.ALIGN_JUSTIFIED
                    };

                    // Agregar el párrafo al documento
                    document.Add(parrafo);

                    List listaNumerada = new List(List.ORDERED)
                    {
                        IndentationLeft = 20f // Agregar margen a la izquierda
                        
                    };

                    Font fontTexto = new Font(Font.FontFamily.HELVETICA, 12);
                    Font boltTexto = new Font(Font.FontFamily.HELVETICA, 12,Font.BOLD);

                    // Agregar los elementos de la lista
                    var item1 = new ListItem ("La credencial entregada al funcionario para el manejo del correo Institucional y Quipux, es para uso institucional e intransferible, y su utilización es de exclusiva responsabilidad del funcionario.", fontTexto);
                    item1.Alignment = Element.ALIGN_JUSTIFIED;
                    listaNumerada.Add(item1);
                    var item2 = new ListItem();
                    item2.Add(new Chunk("El funcionario ", fontTexto));
                    item2.Add(new Chunk(equipoSeleccionado.nombre, boltTexto));
                    item2.Add(new Chunk(", se compromete a la no divulgación y buen uso de la información facilitada por la institución con total confidencialidad, de incumplir con este compromiso será responsable de las consecuencias establecidas en el artículo 190.-“Apropiación fraudulenta por medios electrónicos” del COIP.", fontTexto));
                    item2.Alignment = Element.ALIGN_JUSTIFIED;
                    listaNumerada.Add(item2);
                    var item3 = new ListItem("En caso de pérdida, olvido o sustracción del usuario y/o clave de acceso para el manejo de correo Institucional y Quipux, el funcionario deberá comunicar al área de tecnología del Consejo Nacional Electoral Delegación Pastaza, de manera inmediata.", fontTexto);
                    item3.Alignment = Element.ALIGN_JUSTIFIED;
                    listaNumerada.Add(item3);
                    var item4 = new ListItem("Las credenciales de acceso serán entregadas de manera personal al funcionario responsable de la misma.", fontTexto);
                    item4.Alignment = Element.ALIGN_JUSTIFIED;
                    listaNumerada.Add(item4);

                    document.Add(listaNumerada);

                    Paragraph parrafo3 = new Paragraph($"Para la constancia de lo  actuado y en fe de conformidad y aceptación, se suscribe la presente acta en dos originales de  igual valor y efecto para las personas que  intervienen en esta diligencia, en  la ciudad de Puyo, a los {equipoSeleccionado.fecha:dd} días del mes {equipoSeleccionado.fecha:MMMM} del {equipoSeleccionado.fecha:yyyy}\n", new Font(Font.FontFamily.HELVETICA, 12))
                    {
                        Alignment = Element.ALIGN_JUSTIFIED
                    };

                    document.Add(parrafo3);

                    Paragraph subtituloActa = new Paragraph("CUENTA DE CORREO INSTITUCIONAL:\n", new Font(Font.FontFamily.HELVETICA, 15, Font.BOLD))
                    {
                        Alignment = Element.ALIGN_LEFT
                    };
                    document.Add(subtituloActa);
                    document.Add(new Paragraph($"LINK: mail.cne.gob.ec", new Font(Font.FontFamily.HELVETICA, 12)));
                    document.Add(new Paragraph($"SU CUENTA ES LA SIGUIENTE: {equipoSeleccionado.email}", new Font(Font.FontFamily.HELVETICA, 12)));
                    document.Add(new Paragraph($"COTRASEÑA TEMPORAL: {equipoSeleccionado.tempPass}", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)));
                    document.Add(new Paragraph($"CUENTA  DE QUIPUX: ingresara a su cuenta de  correo institucional y pinchara en el link que le indica para la generación de la clave de Quipux. {equipoSeleccionado.tempPass}", new Font(Font.FontFamily.HELVETICA, 12)));
                    document.Add(new Paragraph("El link para el ingreso a  QUIPUX es el siguiente: quipux.cne.gob.ec, con su  número de  cédula y la contraseña que Ud. genere personalmente.\n\n", new Font(Font.FontFamily.HELVETICA, 12)));

                    PdfPTable tableRef = new PdfPTable(1);
                    tableRef.WidthPercentage = 100;

                    tableRef.AddCell("RECIBIDO POR");
                    document.Add(tableRef);

                    // Tabla de Detalle del Equipo
                    PdfPTable table = new PdfPTable(3);
                    table.WidthPercentage = 100;

                    // Encabezados
                    table.AddCell("NOMBRE");
                    table.AddCell("CARGO");
                    table.AddCell("FIRMA");

                    // Agregar los datos de los equipos

                        table.AddCell(equipoSeleccionado.nombre);
                        table.AddCell(equipoSeleccionado.cargo);
                        table.AddCell(" ");

                    document.Add(table);

                    PdfPTable tableEntref = new PdfPTable(1);
                    tableEntref.WidthPercentage = 100;

                    tableEntref.AddCell("ENTREGADO POR");
                    document.Add(tableEntref);

                    PdfPTable table2 = new PdfPTable(3);
                    table2.WidthPercentage = 100;

                    // Encabezados
                    table2.AddCell("ING. NELSON RICARDO CARDENAS HERMOZA\n\n");
                    table2.AddCell("Técnico Electoral 2\n\n");
                    table2.AddCell(" ");
                    document.Add(table2);

                    document.Close();

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el acta en PDF", ex);
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.Personal.Where(x => ids.Contains(x.id));

                foreach (var item in itemsDelete)
                {
                    item.borrado = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        private static List<ReporteVMR> GenerarDatosReporte()
        {
            var fechaLimite = DateTime.Now.AddMonths(-6);
            var añoActual = DateTime.Now.Year;

            // Obtener todos los equipos
            var equipos = ObtenerEquiposConCustodio(new List<long>())
                .Where(e =>
                    (e.Fecha >= fechaLimite && e.Fecha.Year == añoActual) ||
                    (e.FechaDevolucion >= fechaLimite && e.FechaDevolucion.Value.Year == añoActual))
                .ToList();

            var sistemas = ObtenerPersonal(new List<long>())
                .Where(s => s.fecha >= fechaLimite && s.fecha.Value.Year == añoActual)
                .ToList();

            var reporte = new List<ReporteVMR>();

            // Equipos
            foreach (var equipo in equipos)
            {
                reporte.Add(new ReporteVMR
                {
                    Fecha = equipo.Fecha,
                    Entrega = "NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL",
                    Recibe = equipo.NombreCustodio1 ?? "",
                    EquiposE = "X",
                    Observacion = $"{equipo.nombre_dispositivo}, Marca:{equipo.Marca}, Modelo:{equipo.Modelo}, Serie:{equipo.CodigoCNE}"
                });

                if (equipo.FechaDevolucion != null)
                {
                    reporte.Add(new ReporteVMR
                    {
                        Fecha = equipo.FechaDevolucion,
                        Entrega = equipo.NombreCustodio1 ?? "",
                        Recibe = "NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL",
                        EquiposE = "X",
                        Observacion = $"{equipo.nombre_dispositivo}, Marca:{equipo.Marca}, Modelo:{equipo.Modelo}, Serie:{equipo.CodigoCNE}"
                    });
                }
            }

            // Sistemas
            foreach (var sistema in sistemas)
            {
                reporte.Add(new ReporteVMR
                {
                    Fecha = sistema.fecha,
                    Entrega = "NELSON RICARDO CARDENAS HERMOZA-TECNICO ELECTORAL",
                    Recibe = sistema.nombre,
                    EquiposP = "X",
                    Observacion = "Credenciales Zimbra y Quipux"
                });
            }

            return reporte;
        }




        public static List<ActasMVR> ObtenerEquiposConCustodio(List<long> id_activo = null)
        {
            using (var db = DbConexion.Create())
            {
                var query = from ga in db.gestion_activos
                            where !(bool)ga.borrado
                            select new ActasMVR
                            {
                                Id = ga.id,
                                id_equipo = ga.id_equipo,
                                Fecha = (DateTime)ga.fecha_asignacion,
                                FechaDevolucion = ga.fecha_devolucion,
                                Descripcion = ga.gestion_hardware.observacion,
                                nombre_dispositivo = ga.gestion_hardware.nombre_dispositivo,
                                Marca = ga.gestion_hardware.marca,
                                Modelo = ga.gestion_hardware.modelo,
                                CodigoCNE = ga.gestion_hardware.codigo_cne,
                                Estado = ga.gestion_hardware.estado,
                                NombreCustodio1 = (from hc in db.Custodios
                                                   where hc.id == ga.id_custodio
                                                   select hc.nombre).FirstOrDefault()
                            };

                if (id_activo != null && id_activo.Any())
                {
                    query = query.Where(ga => id_activo.Contains(ga.Id));
                }

                return query.ToList();
            }
        }


        public static async Task<byte[]> DescargarPDF()
        {
            try
            {
                var reporte = GenerarDatosReporte(); // Generar datos del reporte

                using (var stream = new MemoryStream())
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 30, 30);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();

                    // Descargar el logo desde el enlace
                    string logoUrl = "https://i.postimg.cc/BQM89b61/logo2.png";
                    Image logo = null;
                    using (var client = new HttpClient())
                    {
                        var response = await client.GetAsync(logoUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var logoBytes = await response.Content.ReadAsByteArrayAsync();
                            using (var logoStream = new MemoryStream(logoBytes))
                            {
                                logo = Image.GetInstance(logoStream);
                                logo.ScaleToFit(80, 80); // Ajusta el tamaño del logo
                            }
                        }
                    }

                    // Crear tabla principal para el encabezado
                    PdfPTable mainHeaderTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100
                    };
                    mainHeaderTable.SetWidths(new float[] { 1, 3 }); // Configuración de anchos relativos

                    // Subtabla para el logo
                    PdfPTable logoTable = new PdfPTable(1);
                    logoTable.WidthPercentage = 100;

                    if (logo != null)
                    {
                        PdfPCell logoCell = new PdfPCell(logo)
                        {
                            
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 5
                        };
                        logoTable.AddCell(logoCell);
                    }
                    else
                    {
                        // Espacio vacío si no hay logo
                        logoTable.AddCell(new PdfPCell { Border = PdfPCell.NO_BORDER });
                    }

                    // Agregar la subtabla del logo a la tabla principal
                    PdfPCell logoTableCell = new PdfPCell(logoTable)
                    {
                        Border = PdfPCell.NO_BORDER
                    };
                    mainHeaderTable.AddCell(logoTableCell);

                    // Subtabla para los textos
                    PdfPTable textTable = new PdfPTable(1);
                    textTable.WidthPercentage = 100;

                    // Primera línea de texto
                    PdfPCell firstTextCell = new PdfPCell(new Phrase("Reporte de entrega y recepción de equipos informáticos y sistemas electorales para usuarios finales en el ámbito provincial", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)))
                    {
                        
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 5
                    };
                    textTable.AddCell(firstTextCell);

                    // Segunda línea de texto (dinámico)
                    DateTime fechaReferencia = DateTime.Now;
                    DateTime primerMes = new DateTime(fechaReferencia.Year, 1, 1);
                    DateTime segundoMes = primerMes.AddMonths(5);
                    string rangoMeses = $"{primerMes.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"))} - {segundoMes.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"))} {fechaReferencia.Year}";

                    PdfPCell secondTextCell = new PdfPCell(new Phrase($"REGISTRO DE ACTAS ENTREGA RECEPCIÓN\n{rangoMeses}", new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD)))
                    {
                        
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 5
                    };
                    textTable.AddCell(secondTextCell);

                    // Agregar la subtabla de texto a la tabla principal
                    PdfPCell textTableCell = new PdfPCell(textTable)
                    {
                        Border = PdfPCell.NO_BORDER
                    };
                    mainHeaderTable.AddCell(textTableCell);

                    // Agregar la tabla principal al documento
                    pdfDoc.Add(mainHeaderTable);

                    // Tabla para el contenido
                    PdfPTable table = new PdfPTable(6)
                    {
                        WidthPercentage = 100
                    };
                    table.SetWidths(new float[] { 1.2f, 3f, 3f, 0.9f, 0.98f, 2f});

                    var headers = new[] { "Fecha", "Entrega", "Recibe", "Equipos", "Sistemas", "Observación" };
                    foreach (var header in headers)
                    {
                        table.AddCell(new PdfPCell(new Phrase(header, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))
                        {
                            BackgroundColor = BaseColor.LIGHT_GRAY,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        });
                    }

                    string calibriFontPath = "D:\\calibrib.ttf";
                    BaseFont calibriBaseFont = BaseFont.CreateFont(calibriFontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font smallFont = new Font(calibriBaseFont, 11, Font.NORMAL);

                    foreach (var item in reporte)
                    {
                        table.AddCell(new PdfPCell(new Phrase(item.Fecha?.ToString("yyyy-MM-dd") ?? string.Empty, smallFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.Entrega, smallFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.Recibe, smallFont)));
                        table.AddCell(new PdfPCell(new Phrase(item.EquiposE, smallFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER // Centramos el contenido
                        });
                        table.AddCell(new PdfPCell(new Phrase(item.EquiposP, smallFont))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER // Centramos el contenido
                        });
                        table.AddCell(new PdfPCell(new Phrase(item.Observacion, smallFont)));
                    }



                    pdfDoc.Add(table);

                    // Tabla final para "ELABORADO POR"
                    PdfPTable elaboradoTable = new PdfPTable(2)
                    {
                        WidthPercentage = 100 // Ocupa el 100% del ancho de la página
                    };
                    elaboradoTable.SetWidths(new float[] { 2, 1 }); // Configuración de anchos relativos (más ancho para la izquierda)

                    // Celda izquierda con el contenido
                    PdfPCell elaboradoCell = new PdfPCell
                    {
                        Border = PdfPCell.BOX, // Bordes habilitados
                        HorizontalAlignment = Element.ALIGN_CENTER, // Centrar horizontalmente
                        VerticalAlignment = Element.ALIGN_CENTER,   // Centrar verticalmente
                        Padding = 10 // Opcional: agregar algo de espacio interno
                    };

                    // Contenido del texto centrado
                    var contenidoCentrado = new Paragraph
                    {
                        Alignment = Element.ALIGN_CENTER // Asegura que todo el texto dentro del párrafo esté centrado
                    };
                    contenidoCentrado.Add(new Chunk("ELABORADO POR:\n", new Font(calibriBaseFont, 12, Font.BOLD)));
                    contenidoCentrado.Add(new Chunk("ING. RICARDO CARDENAS H.\n", new Font(calibriBaseFont, 12)));
                    contenidoCentrado.Add(new Chunk("TECNICO ELECTORAL 2\n", new Font(calibriBaseFont, 12)));
                    contenidoCentrado.Add(new Chunk("Unidad Provincial de Seguridad Informática y Proyectos Tecnológicos Electorales\n", new Font(calibriBaseFont, 12)));
                    contenidoCentrado.Add(new Chunk("de Tungurahua", new Font(calibriBaseFont, 12)));

                    // Agregar el contenido centrado a la celda
                    elaboradoCell.AddElement(contenidoCentrado);

                    // Agregar la celda izquierda a la tabla
                    elaboradoTable.AddCell(elaboradoCell);

                    // Celda derecha vacía con bordes
                    PdfPCell emptyCell = new PdfPCell(new Phrase(" "))
                    {
                        Border = PdfPCell.BOX, // Bordes habilitados
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        VerticalAlignment = Element.ALIGN_MIDDLE,
                        Padding = 10
                    };
                    elaboradoTable.AddCell(emptyCell);

                    // Agregar la tabla final al documento
                    pdfDoc.Add(elaboradoTable);

                    pdfDoc.Close();

                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el PDF", ex);
            }
        }




        public static byte[] DescargarExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var equipos = ObtenerEquiposConCustodio(new List<long>()); // Llamar con lógica adecuada para obtener todos los equipos
            var sistemas = ObtenerPersonal(new List<long>()); // Llamar con lógica adecuada para obtener todo el personal

            // Generar reporte
            var reporte = GenerarDatosReporte();

            using (var package = new ExcelPackage())
            {
                // Crear hoja de trabajo
                var worksheet = package.Workbook.Worksheets.Add("Informe");

                // Encabezados
                worksheet.Cells[1, 1].Value = "Fecha";
                worksheet.Cells[1, 2].Value = "Entrega";
                worksheet.Cells[1, 3].Value = "Recibe";
                worksheet.Cells[1, 4].Value = "Equipos";
                worksheet.Cells[1, 5].Value = "Sistemas";
                worksheet.Cells[1, 6].Value = "Observación";

                // Formato de encabezados
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Llenar datos
                int fila = 2; // Comenzar en la segunda fila
                foreach (var item in reporte)
                {
                    worksheet.Cells[fila, 1].Value = item.Fecha?.ToString("yyyy-MM-dd") ?? string.Empty;
                    worksheet.Cells[fila, 2].Value = item.Entrega;
                    worksheet.Cells[fila, 3].Value = item.Recibe;
                    worksheet.Cells[fila, 4].Value = item.EquiposE;
                    worksheet.Cells[fila, 5].Value = item.EquiposP;
                    worksheet.Cells[fila, 6].Value = item.Observacion;

                    fila++;
                }

                // Ajustar ancho de las columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Retornar el archivo como byte array
                return package.GetAsByteArray();
            }
        }

    }
}
