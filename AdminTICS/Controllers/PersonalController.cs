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
