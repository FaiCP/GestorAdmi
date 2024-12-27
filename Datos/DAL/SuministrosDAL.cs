using Comun.ViewModels;
using Modelo.Modelo;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    public partial class SuministrosDAL
    {
        public static ListadoPaginadoVMR<SuministrosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<SuministrosVMR> resultado = new ListadoPaginadoVMR<SuministrosVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.suministros_remanufacturados.Select(x => new SuministrosVMR
                {
                    id = x.id,
                    id_equipo = x.id_equipo,
                    tipo_suministro = x.tipo_suministro,
                    fecha_retiro = x.fecha_retiro,
                    id_equipoAsignado = x.id_equipoAsignado
                });
                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                            || x.tipo_suministro.Contains(textoBusqueda)
                                            || x.id_equipoAsignado.Contains(textoBusqueda)
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
        public static long Crear(suministros_remanufacturados item)
        {

            using (var db = DbConexion.Create())
            {
                
                var Suministro = new suministros_remanufacturados
                {
                    id_equipo = item.id_equipo,
                    id_equipoAsignado = item.id_equipoAsignado,
                    fecha_retiro = DateTime.Now,
                    tipo_suministro = item.tipo_suministro,
                    
                };
                db.suministros_remanufacturados.Add(Suministro);
                db.SaveChanges();
            }

            return item.id;
        }
        public static void Actualizar(SuministrosVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.suministros_remanufacturados.Find(item.id);
                itemUpdate.id_equipoAsignado = item.id_equipoAsignado;

                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.suministros_remanufacturados.Where(x => ids.Contains(x.id));

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
