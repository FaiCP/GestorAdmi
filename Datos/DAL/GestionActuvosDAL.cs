using System;
using Comun.ViewModels;
using Modelo.Modelos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;

namespace Datos.DAL
{
    public partial class GestionActuvosDAL
    {
        public static ListadoPaginadoVMR<GestionActivosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<GestionActivosVMR> resultado = new ListadoPaginadoVMR<GestionActivosVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.gestion_activos.Where(x => (bool)!x.borrado).Select(x => new GestionActivosVMR
                {
                    id = x.id,
                    id_equipo = x.id_equipo.ToString(),
                    asignado_a = x.asignado_a,
                    custodio = x.custodio,
                    id_departamento = x.id_departamento,

                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                            || x.custodio.Contains(textoBusqueda)

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
        public static long Crear(gestion_activos item)
        {

            using (var db = DbConexion.Create())
            {
                item.borrado = false;
                db.gestion_activos.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        public static void Actualizar(GestionActivosVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.gestion_activos.Find(item.id);
                itemUpdate.asignado_a = item.asignado_a;
                itemUpdate.id_departamento = item.id_departamento;
                itemUpdate.custodio = item.custodio;

                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.gestion_activos.Where(x => ids.Contains(x.id));

                foreach (var item in itemsDelete)
                {
                    item.borrado = true;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

    }
}
