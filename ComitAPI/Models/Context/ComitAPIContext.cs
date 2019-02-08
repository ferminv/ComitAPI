using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ComitAPI.Models;
using MySql.Data.MySqlClient;

namespace ComitAPI.Models
{

    public class ComitAPIContextConfiguration : DbConfiguration
    {
        public ComitAPIContextConfiguration()
        {
            SetDefaultConnectionFactory(new MySqlConnectionFactory());
            SetProviderServices(MySqlProviderInvariantName.ProviderName, new MySqlProviderServices());
        }
    }

    [DbConfigurationType(typeof(ComitAPIContextConfiguration))]
    public class ComitAPIDBContext : DbContext
    {
        public DbSet<PadronTISH> Padron { get; set; }
        public DbSet<PadronData> PadronData { get; set; }

        public ComitAPIDBContext() : base(@"Server=192.168.1.201;Port=3306;Database=comit_api;uid=root;pwd=ms!574;")
        {
            Database.SetInitializer<ComitAPIDBContext>(null);
            Configuration.ProxyCreationEnabled = false;
            //Configuration.LazyLoadingEnabled = false;
            //db.Database.Log = Console.Write;
            //Configuration.AutoDetectChangesEnabled = false;
        }
    }
}