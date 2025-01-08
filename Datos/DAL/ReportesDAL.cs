using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Comun.ViewModels;
using Modelo.Modelos;

namespace Datos.DAL
{
    public partial class ReportesDAL
    {
        // Método para obtener el conteo total de inventario por tipo de dispositivo
        public List<ReporteInventarioVMR> ObtenerInventarioTotalConEF()
        {
            using (var db = DbConexion.Create())
            {
                return db.gestion_hardware // Filtrar los elementos no borrados
                    .GroupBy(h => h.nombre_dispositivo) // Agrupar por nombre del dispositivo
                    .Select(g => new ReporteInventarioVMR
                    {
                        NombreDispositivo = g.Key,
                        Total = g.Count()
                    })
                    .ToList();
            }
        }

        // Método para obtener préstamos y devoluciones por mes
        public List<ReportePrestamosVMR> ObtenerPrestamosPorMesConEF()
        {
            using (var db = new DbConexion())
            {
                var data = db.gestion_activos
                    .Select(a => new
                    {
                        FechaAsignacion = a.fecha_asignacion,
                        FechaDevolucion = a.fecha_devolucion
                    })
                    .ToList(); // Carga los datos en memoria

                // Procesar en memoria
                var resultado = data
                    .GroupBy(x => x.FechaAsignacion.Value.ToString("MMMM", new System.Globalization.CultureInfo("es-ES"))) // Mes en letras en español
                    .Select(g => new ReportePrestamosVMR
                    {
                        Mes = g.Key,
                        TotalPrestamos = g.Count(),
                        TotalDevoluciones = g.Count(x => x.FechaDevolucion.HasValue)
                    })
                    .OrderBy(r => r.Mes)
                    .ToList();

                return resultado;
            }
        }


        // Método para obtener equipos prestados por tipo
        public List<ReportePrestadosVMR> ObtenerEquiposPrestadosPorTipoConEF()
        {
            using (var db = DbConexion.Create())
            {
                // Obtener los datos de la base de datos
                var resultados = db.gestion_hardware
                    .Join(
                        db.gestion_activos.Where(a => a.fecha_devolucion == null),
                        h => h.id_equipo,
                        a => a.id_equipo,
                        (h, a) => new { h.nombre_dispositivo, a.fecha_asignacion }
                    )
                    .GroupBy(x => new
                    {
                        x.nombre_dispositivo,
                        Mes = x.fecha_asignacion.Value.Month,
                        Año = x.fecha_asignacion.Value.Year
                    })
                    .Select(g => new
                    {
                        g.Key.nombre_dispositivo,
                        TotalPrestados = g.Count(),
                        Mes = g.Key.Mes,
                        Año = g.Key.Año
                    })
                    .ToList(); // Ejecutar la consulta y traer los datos a memoria

                // Convertir el número del mes a su representación textual
                var reporteFinal = resultados.Select(r => new ReportePrestadosVMR
                {
                    NombreDispositivo = r.nombre_dispositivo,
                    TotalPrestados = r.TotalPrestados,
                    Mes = new DateTime(r.Año, r.Mes, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-ES")),
                    Año = r.Año
                }).ToList();

                return reporteFinal;
            }
        }

    }
}
