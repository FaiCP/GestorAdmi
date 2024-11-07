using System;
using Comun.ViewModels;
using Modelo.Modelos;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;

namespace Datos.DAL
{
    public partial class HardwareDAL
    {
        public static ListadoPaginadoVMR<HaedwareVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<HaedwareVMR> resultado = new ListadoPaginadoVMR<HaedwareVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.gestion_hardware.Where(x => (bool)!x.borrado).Select(x => new HaedwareVMR
                {
                    id = x.id,
                    id_equipo = x.id_equipo.ToString(),
                    descripcion = x.descripcion,
                    marca = x.marca,
                    modelo = x.modelo,
                    fecha_adquisicion = x.fecha_adquisicion,
                    estado = x.estado,
                    ubicacion = x.ubicacion,
                    
                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                            || x.marca.Contains(textoBusqueda)
                                            
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
        public static long Crear(gestion_hardware item)
        {

            using (var db = DbConexion.Create())
            {
                item.borrado = false;
                db.gestion_hardware.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }

        public static void Actualizar(HaedwareVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.gestion_hardware.Find(item.id);
                itemUpdate.ubicacion = item.ubicacion;
                itemUpdate.descripcion = item.descripcion;
                itemUpdate.estado = item.estado;
                itemUpdate.marca = item.marca;
                itemUpdate.modelo = item.modelo;
                itemUpdate.descripcion = item.descripcion;

                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.gestion_hardware.Where(x => ids.Contains(x.id));

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
