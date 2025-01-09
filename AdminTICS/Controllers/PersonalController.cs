using Comun.ViewModels;
using Logica.BLL;
using Microsoft.AspNetCore.Mvc;
using Modelo.Modelo;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AdminTICS.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Personal")] 
    public class PersonalController : ApiController
    {
        [HttpGet]
        [Route("LeerTodo")]
        public IHttpActionResult LeerTodo(int cantidad, int pagina, string busqueda)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<PersonalVMR>>();

            try
            {
                respuesta.datos = PersonalBLL.LeerTodo(cantidad, pagina, busqueda);
            }
            catch (Exception ex)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajesErrors.Add(ex.Message);
                //respuesta.mensajesErrors.Add(ex.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }

        [HttpGet]
        [Route("GenerarActa/{ids}")]
        public HttpResponseMessage GenerarActa(string ids)
        {
            try
            {
                var idList = ids.Split(',').Select(long.Parse).ToList();
                byte[] pdfBytes = PersonalBLL.GenerarActaPDF(idList);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Acta_Entrega_Recepciòn{DateTime.Now:yyyyMMddHHmmss}.pdf"
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Error al generar el acta: {ex.Message}")
                };
            }
        }


        [HttpGet]
        [Route("GenerarReporte")]
        public HttpResponseMessage DescargarPDF()
        {
            try
            {
                
                byte[] pdfBytes = PersonalBLL.DescargarPDF();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"actas ER {DateTime.Now:yyyyMMddHHmmss}.pdf"
                };

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Error al generar el acta: {ex.Message}")
                };
            }
        }

        [HttpGet]
        [Route("GenerarReporteExel")]
        public HttpResponseMessage GenerarReporteExcel()
        {
            try
            {
                byte[] excelBytes = PersonalBLL.DescargarExcel(); // Asegúrate de que este método genere un archivo Excel
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(excelBytes)
                };

                // Cambiar el tipo de contenido a Excel
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                // Cambiar la extensión del archivo a .xlsx
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Reporte_actas{DateTime.Now:yyyyMMdd}.xlsx"
                };

                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent($"Error al generar el acta: {ex.Message}")
                };
            }
        }

        [HttpPost]
        [Route("Crear")]
        public IHttpActionResult Crear(Personal item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                respuesta.datos = PersonalBLL.Crear(item);
            }
            catch (Exception ex)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajesErrors.Add(ex.Message);
                respuesta.mensajesErrors.Add(ex.ToString());
            }

            return Content(respuesta.codigo, respuesta);

        }

        [HttpDelete]
        [Route("Eliminar/{ids}")]
        public IHttpActionResult Eliminar([FromUri] List<long> ids)
        {
            var respuesta = new RespuestasVMR<bool>();

            try
            {
                PersonalBLL.Eliminar(ids);
                respuesta.datos = true;
            }
            catch (Exception ex)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = false;
                respuesta.mensajesErrors.Add(ex.Message);
                respuesta.mensajesErrors.Add(ex.ToString());
            }

            return Content(respuesta.codigo, respuesta);
        }
    }
}
