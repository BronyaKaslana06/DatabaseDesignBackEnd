using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Context
{
    public class ModelContextDesignFac : IDesignTimeDbContextFactory<ModelContext>
    {
        public ModelContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ModelContext> builder = new DbContextOptionsBuilder<ModelContext>();
            string connStr = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=8.130.9.72)(PORT=1521))" +
                "(CONNECT_DATA=(SERVICE_NAME=ORCL)));User ID=C##CAR;password=TJ123456";
            builder.UseOracle(connStr);
            ModelContext ctx = new ModelContext(builder.Options);
            return ctx;
        }
    }
}
