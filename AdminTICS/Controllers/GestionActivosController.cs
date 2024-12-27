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
    [RoutePrefix("api/GestionActivos")]
    public class GestionActivosController : ApiController
    {

        [HttpGet]
        [Route("LeerTodo")]
        public IHttpActionResult LeerTodo(int cantidad , int pagina , string busqueda)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<GestionActivosVMR>>();

            try
            {
                respuesta.datos = GestionActivosBLL.LeerTodo(cantidad, pagina, busqueda);
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
        [Route("GenerarActa/{ids}")]
        public HttpResponseMessage GenerarActa(string ids)
        {
            try
            {
                var idList = ids.Split(',').Select(long.Parse).ToList();
                byte[] pdfBytes = GestionActivosBLL.GenerarActaPDF(idList);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Acta_{DateTime.Now:yyyyMMddHHmmss}.pdf"
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
        [Route("GenerarDevolucionPDF/{ids}")]
        public HttpResponseMessage GenerarDevolucionPDF(string ids)
        {
            try
            {
                var idList = ids.Split(',').Select(long.Parse).ToList();
                byte[] pdfBytes = GestionActivosBLL.GenerarDevolucionPDF(idList);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(pdfBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"Acta_{DateTime.Now:yyyyMMddHHmmss}.pdf"
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
        public IHttpActionResult Crear(List<gestion_activos> items)
        {
            var respuesta = new RespuestasVMR<List<long?>>();

            try
            {
                respuesta.datos = GestionActivosBLL.Crear(items);
                respuesta.codigo = HttpStatusCode.OK;
            }
            catch (InvalidOperationException ex)
            {
                respuesta.codigo = HttpStatusCode.BadRequest; 
                respuesta.datos = null;
                respuesta.mensajesErrors.Add(ex.Message);
                //respuesta.mensajesErrors.Add(ex.ToString());
            }


            return Content(respuesta.codigo, respuesta);
        }


        [HttpPut]
        [Route("Actualizar/{id}")]
        public IHttpActionResult Actualizar(long id, GestionActivosVMR item)
        {
            var respuesta = new RespuestasVMR<bool>();

            try
            {
                item.id = id;
                GestionActivosBLL.Actualizar(item);
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
                GestionActivosBLL.Eliminar(ids);
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
