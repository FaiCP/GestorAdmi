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
    public partial class SuministrosBLL
    {
        public static ListadoPaginadoVMR<SuministrosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return SuministrosDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
        public static long Crear(suministros_remanufacturados item)
        {
            return SuministrosDAL.Crear(item);
        }
        public static void Actualizar(SuministrosVMR item)
        {
            SuministrosDAL.Actualizar(item);
        }
        public static void Eliminar(List<long> ids)
        {
            SuministrosDAL.Eliminar(ids);
        }
    }
}
