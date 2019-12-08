using System;
using System.IO;
using CatMash.DataStore.EF.SQLite;

namespace EF.Migrations.Helper
{
    public static partial class EFMigrationHelper
    {
        static EFMigrationHelper()
        {
            _DBContext = new CatMashSQLiteDbContextFactory().CreateDbContext(new string[] { Directory.GetCurrentDirectory() + "/Migration.sqlite" });
        }
    }
}
