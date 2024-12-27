using Comun.ViewModels;
using Datos.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    public partial class DepartamentosBLL
    {
        public static ListadoPaginadoVMR<DepartamentosVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return DepartamentosDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
    }
}
