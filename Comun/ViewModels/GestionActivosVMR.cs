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
        public string asignado_a { get; set; }
        public System.DateTime fecha_asignacion { get; set; }
        public System.DateTime fecha_devolucion { get; set; }
        public Nullable<long> id_departamento { get; set; }
        public string custodio { get; set; } = string.Empty;
    }
}
