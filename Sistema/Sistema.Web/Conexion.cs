using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema.Web
{
     static class Conexion
    {

        public static string Conectar()
        {
           string conectar = "server=CA214064\\SQLEXPRESS;Database=dbsistema;Trusted_Connection=True;";
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(conectar);
             builder.ApplicationName = "Conexion";
            return builder.ConnectionString;
        }

    }
}
