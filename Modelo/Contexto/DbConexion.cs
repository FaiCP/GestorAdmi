using Modelo.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    public partial class DbConexion : DbContext
    {
        private DbConexion(string stringConexion)
            : base(stringConexion)
        { 
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Database.CommandTimeout = 900;
        }

        public static DbConexion Create()
        {
            return new DbConexion("name=DbConext");
        }
        public virtual DbSet<control_activos> control_activos { get; set; }
        public virtual DbSet<departamentos> departamentos { get; set; }
        public virtual DbSet<gestion_activos> gestion_activos { get; set; }
        public virtual DbSet<gestion_hardware> gestion_hardware { get; set; }
        public virtual DbSet<historial_custodios> historial_custodios { get; set; }
        public virtual DbSet<historial_mantenimiento> historial_mantenimiento { get; set; }
        public virtual DbSet<suministros_remanufacturados> suministros_remanufacturados { get; set; }
    }
}
