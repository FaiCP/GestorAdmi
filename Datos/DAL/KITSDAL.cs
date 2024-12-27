using Comun.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Modelo.Modelos;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;

namespace Datos.DAL
{
    public partial class KITSDAL
    {
        public static ListadoPaginadoVMR<KitsVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            ListadoPaginadoVMR<KitsVMR> resultado = new ListadoPaginadoVMR<KitsVMR>();

            using (var db = DbConexion.Create())
            {
                var query = db.Kits.Select(x => new KitsVMR
                {
                    id = x.id,
                    ESTADO = x.ESTADO,
                    INSUMO = x.INSUMO,
                    CANTIDAD = x.CANTIDAD,
                    MARCA = x.MARCA,
                    OBSERVACION = x.OBSERVACION,
                    MODELO = x.MODELO,
                    Serie = x.Serie,                    
                });


                if (!string.IsNullOrEmpty(textoBusqueda))
                {
                    query = query.Where(x => x.id_equipo.Contains(textoBusqueda)
                                            || x.MARCA.Contains(textoBusqueda)
                                            || x.INSUMO.Contains(textoBusqueda)
                                            || x.Serie.Contains(textoBusqueda)
                                            || x.MODELO.Contains(textoBusqueda)

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

        public static long Crear(Kits nuevoItem)
        {
            using (var db = DbConexion.Create())
            {
                var KITS = new Kits
                {
                    Serie = nuevoItem.Serie,
                    OBSERVACION = nuevoItem.OBSERVACION,
                    CANTIDAD = nuevoItem.CANTIDAD,
                    ESTADO = nuevoItem.Serie,
                    INSUMO = nuevoItem.INSUMO,
                    MARCA = nuevoItem.MARCA,
                    MODELO = nuevoItem.MODELO
                };
                db.Kits.Add(KITS);
                db.SaveChanges();
            }

            return nuevoItem.id;

        }
        public static void Actualizar(KitsVMR item)
        {
            using (var db = DbConexion.Create())
            {
                var itemUpdate = db.Kits.Find(item.id);
                itemUpdate.Serie = item.Serie;
                itemUpdate.OBSERVACION = item.OBSERVACION;
                itemUpdate.INSUMO = item.INSUMO;
                itemUpdate.CANTIDAD = item.CANTIDAD;
                itemUpdate.MODELO = item.MODELO;
                itemUpdate.MARCA = item.MARCA;
                itemUpdate.MODELO = item.MODELO;
                db.Kits.Add(itemUpdate);

                db.Entry(itemUpdate).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static void Eliminar(List<long> ids)
        {
            using (var db = DbConexion.Create())
            {
                var itemsDelete = db.Kits.Where(x => ids.Contains(x.id));

                foreach (var item in itemsDelete)
                {
                    
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        
    }
}
