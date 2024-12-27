using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class CustodioVMR
    {
        public long id { get; set; } 
        public string departamento { get; set; }
        public string cedula_empleado { get; set; }
        public string cargo_empleado { get; set; }
        public string nombre_empleado { get; set; }
        public long id_departamento { get; set; }
    }
}
