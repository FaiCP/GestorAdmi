using Modelo.Modelo;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Modelos
{
    public partial class DbConexion : DbConext
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
            return new DbConexion("name=DbConexion");
        }
        
    }
}
