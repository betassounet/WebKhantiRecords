using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.BDD {

    public class StaticEntityConnectionStringBuilder {

        static EntityConnectionStringBuilder GetEntityConnectionStringBuilderFromDataSource(string serverName, string databaseName, string UserId, string Password) {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder = new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.UserID = UserId;
            sqlBuilder.Password = Password;
            sqlBuilder.PersistSecurityInfo = true;
            sqlBuilder.MultipleActiveResultSets = true;

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
                new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;
            return entityBuilder;

        }

        public static EntityConnectionStringBuilder GetConnectionStringFromDataSourceAndModelName(string serverName, string databaseName, string ModelName, string UserId, string Password) {
            //Exemple de ModelName: Data.DBModelOrphea
            string Metadata = @"res://*/" + ModelName + @".csdl|"
                            + @"res://*/" + ModelName + @".ssdl|"
                            + @"res://*/" + ModelName + @".msl";
            EntityConnectionStringBuilder entityConnectionStringBuilder = GetEntityConnectionStringBuilderFromDataSource(serverName, databaseName, UserId, Password);
            // Set the Metadata location.
            entityConnectionStringBuilder.Metadata = Metadata;
            return entityConnectionStringBuilder;
        }

    }

}
