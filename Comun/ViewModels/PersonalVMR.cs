using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public partial class PersonalVMR
    {
        public long Id { get; set; }
        public string nombre { get; set; }
        public string cedula { get; set; }
        public string cargo { get; set; }
        public string email { get; set; }
        public string tempPass { get; set; }
        public Nullable<System.DateTime> fecha { get; set; }
    }
}
