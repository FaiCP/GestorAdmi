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
    [RoutePrefix("api/Reportes")]
    public class ReportesController : ApiController
    {
        private readonly ReportesBLL _reportesBll;

        public ReportesController()
        {
            _reportesBll = new ReportesBLL();
        }

        // Endpoint para obtener el total de inventario
        [HttpGet]
        [Route("InventarioTotal")]
        public IHttpActionResult ObtenerInventarioTotal()
        {
            try
            {
                var resultado = _reportesBll.ObtenerInventarioTotal();
                return Ok(resultado);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Endpoint para obtener préstamos y devoluciones por mes
        [HttpGet]
        [Route("PrestamosPorMes")]
        public IHttpActionResult ObtenerPrestamosPorMes()
        {
            try
            {
                var resultado = _reportesBll.ObtenerPrestamosPorMes();
                return Ok(resultado);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // Endpoint para obtener equipos prestados por tipo
        [HttpGet]
        [Route("EquiposPrestadosPorTipo")]
        public IHttpActionResult ObtenerEquiposPrestadosPorTipo()
        {
            try
            {
                var resultado = _reportesBll.ObtenerEquiposPrestadosPorTipo();
                return Ok(resultado);
            }
            catch (System.Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
