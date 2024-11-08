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
    public class HardwareController : ApiController
    {
        [HttpGet]
        public IHttpActionResult LeerTodo(int cantidad = 10, int pagina = 0, string textoBusqueda = null)
        {
            var respuesta = new RespuestasVMR<ListadoPaginadoVMR<HaedwareVMR>>();

            try
            {
                respuesta.datos = HardwareBLL.LeerTodo(cantidad, pagina, textoBusqueda);
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
        public IHttpActionResult Crear(gestion_hardware item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                respuesta.datos = HardwareBLL.Crear(item);
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
        public IHttpActionResult Eliminar(List<long> ids)
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
