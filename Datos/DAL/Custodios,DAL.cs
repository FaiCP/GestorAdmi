using Comun.ViewModels;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    public partial class Custodios_DAL
    {
        public static ListadoPaginadoVMR<CustodioVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<CustodioVMR> resultado = new ListadoPaginadoVMR<CustodioVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.historial_custodios.Select(x => new CustodioVMR
                {
                    id = x.id,
                    cedula_empleado = x.cedula_empleado,
                    cargo_empleado = x.cargo_empelado,
                    nombre_empleado = x.nombre_empleado
                });
                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.cargo_empleado.Contains(textoBusqueda)
                                            || x.nombre_empleado.Contains(textoBusqueda)
                                            || x.cedula_empleado.Contains(textoBusqueda)

                    );
                }

                resultado.cantidadTotal = query.Count();

                resultado.elementos = query
                    .OrderBy(x => x.id)
                    .Skip(pagina * cantidad)
                    .Take(cantidad)
                    .ToList();
            }

            return resultado;
        }
    }
}
