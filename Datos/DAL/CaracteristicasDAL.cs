using Modelo.Modelo;
using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datos.DAL
{
    public partial class CaracteristicasDAL
    {
        public static long Crear(caracteristicas_computadora item)
        {

            using (var db = DbConexion.Create())
            {
                db.caracteristicas_computadora.Add(item);
                db.SaveChanges();
            }

            return item.id;
        }
    }
}
