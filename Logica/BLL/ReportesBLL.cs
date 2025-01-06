using Comun.ViewModels;
using Datos.DAL;
using Modelo.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    public partial class ReportesBLL
    {
        private readonly ReportesDAL _reportesDal;

        public ReportesBLL()
        {
            _reportesDal = new ReportesDAL();
        }
        public List<ReporteInventarioVMR> ObtenerInventarioTotal()
        {
            return _reportesDal.ObtenerInventarioTotalConEF();
        }

        // Método para obtener préstamos y devoluciones por mes
        public List<ReportePrestamosVMR> ObtenerPrestamosPorMes()
        {
            return _reportesDal.ObtenerPrestamosPorMesConEF();
        }

        // Método para obtener equipos prestados por tipo
        public List<ReportePrestadosVMR> ObtenerEquiposPrestadosPorTipo()
        {
            return _reportesDal.ObtenerEquiposPrestadosPorTipoConEF();
        }
    }
}
