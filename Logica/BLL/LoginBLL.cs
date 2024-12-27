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
    public partial class LoginBLL
    {
        public static long Crear(Usuarios item)
        {
            return LoginDAL.CrearUsuario(item);
        }
    }
}
