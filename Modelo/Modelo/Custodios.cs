//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Modelo.Modelo
{
    using System;
    using System.Collections.Generic;
    
    public partial class Custodios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Custodios()
        {
            this.gestion_activos = new HashSet<gestion_activos>();
        }
    
        public string nombre { get; set; }
        public string cargo { get; set; }
        public string cedula { get; set; }
        public long id { get; set; }
        public Nullable<long> id_departamento { get; set; }
    
        public virtual departamentos departamentos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gestion_activos> gestion_activos { get; set; }
    }
}
