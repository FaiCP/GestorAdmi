using Modelo.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Metadatos
{
    [MetadataType(typeof(HistorialesMetadato))]
    public partial class Historiales
    {
    }
    public class HistorialesMetadato
    {
        public Nullable<System.DateTimeOffset> marca_temporal { get; set; }
        public string custodio { get; set; }
        public long id_equipo { get; set; }
        public Nullable<System.DateTime> fecha_mantenimiento { get; set; }
        public string detalles { get; set; }
    }
}
