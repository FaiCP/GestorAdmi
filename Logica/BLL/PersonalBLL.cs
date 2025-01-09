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
    public partial class PersonalBLL
    {
        public static ListadoPaginadoVMR<PersonalVMR> LeerTodo(int cantidad, int pagina, string textoBusqueda)
        {
            return PersonalDAL.LeerTodo(cantidad, pagina, textoBusqueda);
        }
        public static byte[] GenerarActaPDF(List<long> id)
        {
            return PersonalDAL.GenerarActaPDF(id);
        }

        public static long Crear(Personal item)
        {
            return PersonalDAL.Crear(item);
        }
        public static void Eliminar(List<long> ids)
        {
            PersonalDAL.Eliminar(ids);
        }

        public static byte[] DescargarPDF()
        {
            return PersonalDAL.DescargarPDF();
        }
        public static byte[] DescargarExcel()
        {
            return PersonalDAL.DescargarExcel();
        }
    }
}
