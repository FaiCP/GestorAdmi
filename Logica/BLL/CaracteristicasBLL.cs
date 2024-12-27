using Datos.DAL;
using Modelo.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.BLL
{
    public partial class CaracteristicasBLL
    {
        public static long Crear(caracteristicas_computadora item)
        {
            return CaracteristicasDAL.Crear(item);
        }
    }
}
