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
    
    public partial class departamentos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public departamentos()
        {
            this.gestion_activos = new HashSet<gestion_activos>();
            this.historial_custodios = new HashSet<historial_custodios>();
            this.gestion_activos1 = new HashSet<gestion_activos>();
        }
    
        public long id { get; set; }
        public string nombre { get; set; }
        public string nombre_empleado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gestion_activos> gestion_activos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<historial_custodios> historial_custodios { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<gestion_activos> gestion_activos1 { get; set; }
    }
}