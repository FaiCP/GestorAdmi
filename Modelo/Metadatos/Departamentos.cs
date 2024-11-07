using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    [MetadataType(typeof(DepartamentosMetadato))]
    public partial class Departamentos
    {
    }
    public class DepartamentosMetadato
    {
        public string nombre { get; set; }
        public string nombre_empleado { get; set; }
    }
}
