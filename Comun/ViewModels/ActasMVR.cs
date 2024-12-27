using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comun.ViewModels
{
    public class ActasMVR
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public string id_equipo { get; set; }
        public string nombre_dispositivo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string CodigoCNE { get; set; }
        public string Estado { get; set; }
        public string Departamento { get; set; }
        public string NombreCustodio { get; set; } 
        public string NombreCustodio1 { get; set; }
        public string cargo1 { get; set; }
        public string cargo2 { get; set; }
        public DateTime Fecha { get; set; }
        public Nullable<decimal> valor { get; set; }

    }
    }
