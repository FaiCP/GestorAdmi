using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class SuministrosVMR
    {
        public long id { get; set; }
        public string id_equipo { get; set; }
        public string tipo_suministro { get; set; }
        public string id_equipoAsignado { get; set; }
        public Nullable<System.DateTime> fecha_retiro { get; set; }

    }
}
