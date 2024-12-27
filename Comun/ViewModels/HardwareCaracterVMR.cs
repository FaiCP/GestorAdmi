using Modelo.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class HardwareCaracterVMR
    {
        public long id { get; set; }
        public long idcaja { get; set; }
        public string ubicacion { get; set; }
        public string descripcion { get; set; }
        public string nombre_dispositivo { get; set; }
        public string CANTIDAD { get; set; }
        public string marca { get; set; }
        public string modelo { get; set; }
        public string codigo_cne { get; set; }
        public Nullable<System.DateTime> fecha_adquisicion { get; set; }
        public string id_equipo { get; set; }
        public string estado { get; set; }
        public string ram { get; set; }
        public string rom { get; set; }
        public string procesador { get; set; }
        public Nullable<decimal> valor { get; set; }
        public Nullable<bool> borrado { get; set; }

    }
}
