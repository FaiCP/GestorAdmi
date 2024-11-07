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
    public class HardwareBLL
    {
        public static ListadoPaginadoVMR<HaedwareVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return HardwareDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
        public static long Crear(gestion_hardware item)
        {
            return HardwareDAL.Crear(item);
        }
        public static void Actualizar(HaedwareVMR item)
        {
            HardwareDAL.Actualizar(item);
        }
        public static void Eliminar(List<long> ids)
        {
            HardwareDAL.Eliminar(ids);
        }
    }
}
