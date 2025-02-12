using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class ReporteVMR
    {
        public Nullable<System.DateTime> Fecha { get; set; }
        public string Entrega { get; set; }
        public string Recibe { get; set; }
        public string EquiposE { get; set; }
        public string EquiposP { get; set; }
        public string Observacion { get; set; }

    }
}
