using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    [MetadataType(typeof(HardwareMetadato))]
    public partial class Hardware 
    {
    }

    public class HardwareMetadato
    {
        public string descripcion { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public Nullable<System.DateTime> fecha_adquisicion { get; set; }
        public string estado { get; set; }
        public string ubicacion { get; set; }
    }
}