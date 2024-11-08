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
    public class GestionActivosController : ApiController
    {
        [HttpGet]
        public IHttpActionResult LeerTodo(int cantidad = 10, int pagina = 0, string textoBusqueda = null)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<GestionActivosVMR>>();

            try
            {
                respuesta.datos = GestionActivosBLL.LeerTodo(cantidad, pagina, textoBusqueda);
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
        public IHttpActionResult Crear(gestion_activos item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                respuesta.datos = GestionActivosBLL.Crear(item);
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
        public IHttpActionResult Eliminar(List<long> ids)
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
