using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class EntradaActivosVMR
    {
        public long id { get; set; }
        public string id_equipo { get; set; }
        public long id_custodio { get; set; }
        public long id_departamento { get; set; }
        public DateTime Fecha { get; set; } 
    }
}
