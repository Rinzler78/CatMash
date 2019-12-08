using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CatMash.DataStore.EF.SQLite
{
    public class CatMashSQLiteDbContextFactory : IDesignTimeDbContextFactory<CatMashSQLiteDbContext>
    {
        public CatMashSQLiteDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CatMashSQLiteDbContext>();

            var connectionString = (args?.Length ?? 0) == 0 ? "DB.sqlite" : args[0];
            optionsBuilder.UseSqlite(connectionString);

            return new CatMashSQLiteDbContext(optionsBuilder.Options);
        }
    }
}
