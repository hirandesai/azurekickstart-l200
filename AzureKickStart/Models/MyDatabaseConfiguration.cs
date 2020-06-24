using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace DotNetAppSqlDb.Models
{
    public class MyDatabaseConfiguration : DbConfiguration
    {
        public MyDatabaseConfiguration()
        {
            // https://docs.microsoft.com/en-us/ef/ef6/fundamentals/connection-resiliency/retry-logic
            //SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
            //To set the maximum number of retries to 1 and the maximum delay to 5 seconds
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy(1, TimeSpan.FromSeconds(5)));
        }
    }
}