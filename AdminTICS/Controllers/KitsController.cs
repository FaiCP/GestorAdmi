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
    [RoutePrefix("api/Kits")]
    public class KitsController : ApiController
    {
        [HttpGet]
        [Route("LeerTodo")]
        public IHttpActionResult LeerTodo(int cantidad, int pagina, string busqueda)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<KitsVMR>>();

            try
            {
                respuesta.datos = KitsBLL.LeerTodo(cantidad, pagina, busqueda);
            }
            catch (Exception ex)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = null;
                respuesta.mensajesErrors.Add(ex.Message);
            }

            return Content(respuesta.codigo, respuesta);
        }

        [HttpPost]
        [Route("Crear")]
        public IHttpActionResult Crear(KitsVMR item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                var id = KitsBLL.Crear(item);
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
        public IHttpActionResult Actualizar(long id, KitsVMR item)
        {
            var respuesta = new RespuestasVMR<bool>();

            try
            {
                item.id = id;
                KitsBLL.Actualizar(item);
                respuesta.datos = true;
            }
            catch (Exception ex)
            {
                respuesta.codigo = HttpStatusCode.InternalServerError;
                respuesta.datos = false;
                respuesta.mensajesErrors.Add(ex.Message);
            }

            return Content(respuesta.codigo, respuesta);
        }

        [HttpGet]
        [Route("GenerarActa")]
        public HttpResponseMessage GenerarActa()
        {
            try
            {
                byte[] pdfBytes = KitsBLL.GenerarActaPDF();
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
    }
}
