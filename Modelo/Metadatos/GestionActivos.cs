using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    [MetadataType(typeof(GestionActivosMetadato))]
    public partial class GestionActivosMetadato
    {
        public string asignado_a { get; set; }
        public Nullable<System.DateTime> fecha_asignacion { get; set; }
        public Nullable<System.DateTime> fecha_devolucion { get; set; }
        public Nullable<long> id_departamento { get; set; }
        public string custodio { get; set; } = string.Empty;    
    }
}
