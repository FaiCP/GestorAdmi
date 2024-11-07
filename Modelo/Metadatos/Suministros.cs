using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    [MetadataType(typeof(SuministrosMetadato))]
    public partial class SuministrosMetadato
    {
        public Nullable<long> id_equipo { get; set; }
        public string tipo_suministro { get; set; }
        public Nullable<System.DateTime> fecha_retiro { get; set; }

    }
}
