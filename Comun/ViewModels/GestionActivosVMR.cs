using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class GestionActivosVMR
    {
        public long id { get; set; }
        public string id_equipo { get; set; }
        public long id_custodio { get; set; }
        public Nullable<System.DateTime> fecha_asignacion { get; set; }
        public Nullable<System.DateTime> fecha_devolucion { get; set; }
        public Nullable<long> id_departamento { get; set; }
        public string nombre_empleado { get; set; }
        public Nullable<bool> borrado { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public Nullable<System.DateTime> fecha_adquisicion { get; set; }
        public string estado { get; set; }
        public string ubicacion { get; set; }
        public string codigo_cne { get; set; }
        public string nombre_dispositivo { get; set; }
        public HaedwareVMR haedware { get; set; }
        public CaracteristicasHarrdwareVMR caracteristicas { get; set; }
    }
}
