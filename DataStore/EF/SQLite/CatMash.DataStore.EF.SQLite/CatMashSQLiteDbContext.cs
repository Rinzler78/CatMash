using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;

namespace CatMash.DataStore.EF.SQLite
{
    public static class DBContextExt
    {
        public static DbContextOptions GetOptions(this string dbPath)
        {
            return SqliteDbContextOptionsBuilderExtensions.UseSqlite(new DbContextOptionsBuilder(), dbPath).Options;
        }
    }

    public class CatMashSQLiteDbContext : CatMashDbContext
    {
        public CatMashSQLiteDbContext()
        {
            InitSqLite();
        }

        public CatMashSQLiteDbContext(DbContextOptions options) : base(options)
        {
            InitSqLite();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            SqliteOptionsExtension option = optionsBuilder.Options.FindExtension<SqliteOptionsExtension>();

            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string dbfileName = option?.ConnectionString ?? GetType().Name + ".sqlite";
            string fullPath = Path.Combine(documentsPath, dbfileName);

            FileInfo file = new FileInfo(fullPath);

            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            Console.WriteLine($"Using SQLite file {file.FullName}");

            optionsBuilder.UseSqlite($"Filename={fullPath}");
        }

        private void InitSqLite()
        {
            try
            {
                SQLitePCL.Batteries_V2.Init();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"InitSqLite Exeption : {ex}");
                throw ex;
            }
        }
    }
}
