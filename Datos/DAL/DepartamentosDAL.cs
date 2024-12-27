using Comun.ViewModels;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;

namespace Datos.DAL
{
    public partial class DepartamentosDAL
    {
        public static ListadoPaginadoVMR<DepartamentosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<DepartamentosVMR> resultado = new ListadoPaginadoVMR<DepartamentosVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.departamentos.Select(x => new DepartamentosVMR
                {
                    id = x.id,
                    nombre= x.nombre,
                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.nombre.Contains(textoBusqueda)
                                            || x.nombre_empleado.Contains(textoBusqueda)

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
