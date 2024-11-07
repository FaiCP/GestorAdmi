using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class HaedwareVMR
    {
        public long id { get; set; }
        public string id_equipo { get; set; }
        public string descripcion { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public Nullable<System.DateTime> fecha_adquisicion { get; set; }
        public string estado { get; set; }
        public string ubicacion { get; set; }
    }
}
