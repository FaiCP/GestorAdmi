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
    public class DepartamentosController : ApiController
    {
            [HttpGet]
            public IHttpActionResult LeerTodo(int cantidad = 10, int pagina = 0, string busqueda = null)
            {
                var respuesta = new RespuestasVMR<ListadoPaginadoVMR<DepartamentosVMR>>();

                try
                {
                    respuesta.datos = DepartamentosBLL.LeerTodo(cantidad, pagina, busqueda);
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
