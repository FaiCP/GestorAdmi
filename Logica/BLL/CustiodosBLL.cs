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
    public partial class CustiodosBLL
    {
        public static ListadoPaginadoVMR<CustodioVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return CustodiosDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
        public static long Crear(Custodios item)
        {
            return CustodiosDAL.Crear(item);
        }

        public static byte[] GenerarActaPDF()
        {
            return CustodiosDAL.GenerarActaPDF();
        }
    }
}
