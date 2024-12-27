using System;
using Comun.ViewModels;
using Modelo.Modelos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modelo.Modelo;
using System.Reflection;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;

namespace Datos.DAL
{
    public partial class LoginDAL
    {
        
        public static long CrearUsuario(Usuarios item)
        {
            using (var db = DbConexion.Create())
            {
                db.Usuarios.Add(item);
                db.SaveChanges();
            }
            return item.id;
        }
    }
}
