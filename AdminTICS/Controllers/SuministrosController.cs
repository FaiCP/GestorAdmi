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
    [RoutePrefix("api/Suministros")]
    public class SuministrosController : ApiController
    {
        [HttpGet]
        [Route("LeerTodo")]
        public IHttpActionResult LeerTodo(int cantidad, int pagina, string busqueda )
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<SuministrosVMR>>();

            try
            {
                respuesta.datos = SuministrosBLL.LeerTodo(cantidad, pagina, busqueda);
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

        [HttpPost]
        [Route("Crear")]
        public IHttpActionResult Crear(suministros_remanufacturados item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                respuesta.datos = SuministrosBLL.Crear(item);
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
        public IHttpActionResult Actualizar(long id, SuministrosVMR item)
        {
            var respuesta = new RespuestasVMR<bool>();

            try
            {
                item.id = id;
                SuministrosBLL.Actualizar(item);
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
                SuministrosBLL.Eliminar(ids);
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
