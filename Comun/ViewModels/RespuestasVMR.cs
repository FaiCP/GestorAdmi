using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class RespuestasVMR<T>
    {
        public HttpStatusCode codigo { get; set; }

        public T datos { get; set; }

        public List<string> mensajesErrors { get; set; }

        public RespuestasVMR()
        {
            codigo = HttpStatusCode.OK;
            datos = default(T);
            mensajesErrors = new List<string>();
        }
    }
}
