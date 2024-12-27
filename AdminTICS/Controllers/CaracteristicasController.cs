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
    public class CaracteristicasController : ApiController
    {

        [HttpPost]
        public IHttpActionResult Crear(caracteristicas_computadora item)
        {
            var respuesta = new RespuestasVMR<long?>();

            try
            {
                if (item == null || string.IsNullOrEmpty(item.id_equipo))
                {
                    return BadRequest("Los datos de las características no son válidos.");
                }
                respuesta.datos = CaracteristicasBLL.Crear(item);
                return Ok("Características agregadas");
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
    }
}
