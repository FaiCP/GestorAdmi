using Comun.ViewModels;
using Datos.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    public partial class HistorialPrestamosBLL
    {
        public static ListadoPaginadoVMR<HistorialPrestamosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda, int? idCustodio)
        {
            return HistorialPrestamosDAL.LeerTodo(cantidad, pagina, textoBusqueda, idCustodio);
        }
        public static byte[] GenerarActaPDF(List<long> id)
        {
            return HistorialPrestamosDAL.GenerarActaPDF(id);
        }
    }
}
