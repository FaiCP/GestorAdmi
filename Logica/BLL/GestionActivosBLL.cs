using Comun.ViewModels;
using Datos.DAL;
using Modelo.Modelo;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    public class GestionActivosBLL
    {
        public static ListadoPaginadoVMR<GestionActivosVMR> LeerTodo(int cantidad, int pagina, string busqueda)
        {
            return GestionActuvosDAL.LeerTodo(cantidad, pagina, busqueda);
        }
        public static byte[] GenerarActaPDF(List<long> id )
        {
            return GestionActuvosDAL.GenerarActaPDF(id);
        }

        public static byte[] GenerarDevolucionPDF(List<long> id)
        {
            return GestionActuvosDAL.GenerarDevolucionPDF(id);
        }

        public static List<long?> Crear(List<gestion_activos> items)
        {
            return GestionActuvosDAL.Crear(items);
        }

        public static void Actualizar(GestionActivosVMR item)
        {
            GestionActuvosDAL.Actualizar(item);
        }
        public static void Eliminar(List<long> ids)
        {
            GestionActuvosDAL.Eliminar(ids);
        }
    }
}
