using Comun.ViewModels;
using Logica.BLL;
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
    [RoutePrefix("api/Hardware")]
    public class HardwareController : ApiController
    {
        [HttpOptions]
        [Route("")]
        public IHttpActionResult Options()
        {
            return Ok();
        }

        [HttpGet]
        [Route("LeerTodo")]
        public IHttpActionResult LeerTodo(int cantidad, int pagina , string busqueda)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<HaedwareVMR>>();

            try
            {
                respuesta.datos = HardwareBLL.LeerTodo(cantidad, pagina, busqueda);
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

        [HttpGet]
        [Route("GenerarActa")]
        public HttpResponseMessage GenerarActa()
        {
            try
            {
                byte[] pdfBytes = HardwareBLL.GenerarActaPDF();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Acta_Inventario_{DateTime.Now:yyyyMMddHHmmss}.pdf"
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

        [HttpGet]
        [Route("GenerarActaExel")]
        public HttpResponseMessage GenerarActaExcel()
        {
            try
            {
                byte[] excelBytes = HardwareBLL.GenerarActaExcel(); // Asegúrate de que este método genere un archivo Excel
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(excelBytes)
                };

                // Cambiar el tipo de contenido a Excel
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                // Cambiar la extensión del archivo a .xlsx
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Acta_Inventario_{DateTime.Now:yyyyMMdd}.xlsx"
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
        public IHttpActionResult Crear(HardwareCaracterVMR item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                var id = HardwareBLL.Crear(item);
                Console.WriteLine("Iniciando creación de hardware...");
                return Ok(new { id });
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

        [HttpPut]
        [Route("Actualizar/{id}")]
        public IHttpActionResult Actualizar(long id, HaedwareVMR item)
        {
            var respuesta = new RespuestasVMR<bool>();

            try
            {
                item.id = id;
                HardwareBLL.Actualizar(item);
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

        [HttpDelete]
        [Route("Eliminar/{ids}")]
        public IHttpActionResult Eliminar([FromUri] List<long> ids)
        {
            var respuesta = new RespuestasVMR<bool>();

            try
            {
                HardwareBLL.Eliminar(ids);
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
