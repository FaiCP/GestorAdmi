using Comun.ViewModels;
using Datos.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    public partial class KitsBLL
    {
        public static ListadoPaginadoVMR<KitsVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return KITSDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
    }
}
