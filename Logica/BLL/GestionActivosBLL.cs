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
        public static ListadoPaginadoVMR<GestionActivosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return GestionActuvosDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
        public static long Crear(gestion_activos item)
        {
            return GestionActuvosDAL.Crear(item);
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
