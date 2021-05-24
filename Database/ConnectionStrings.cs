using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public static class ConnectionStrings
    {
        public static string DefaultConnection =>
            "server=(localdb)\\MSSQLLocalDB;Initial Catalog=badaboomDb;Integrated Security=True;MultipleActiveResultSets=True;";
            //ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString;
    }
}
