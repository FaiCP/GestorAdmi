using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class KitsVMR
    {
        public long id { get; set; }
        public string INSUMO { get; set; }
        public string CANTIDAD { get; set; }
        public string ESTADO { get; set; }
        public string OBSERVACION { get; set; }
        public string MARCA { get; set; }
        public string Serie { get; set; }
        public string MODELO { get; set; }
        public string id_equipo { get; set; }
    }
}
