using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class HistorialPrestamosVMR
    {
        public long id { get; set; }
        public string nombre_empleado { get; set; }
        public string nombre_dispositivo { get; set; }
        public string nombre_departamento { get; set; }
        public string codigo_cne { get; set; }
        public string id_equipo { get; set; }
        public Nullable<System.DateTime> fecha_asignacion { get; set; }
        public Nullable<System.DateTime> fecha_devolucion { get; set; }
        public HaedwareVMR hardware { get; set; }
    }
}
