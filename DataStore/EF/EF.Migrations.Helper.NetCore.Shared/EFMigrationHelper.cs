using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CatMash.DataStore.EF;

namespace EF.Migrations.Helper
{
    public static partial class EFMigrationHelper
    {
        static CatMashDbContext _DBContext;

        public static async Task<bool> Launch()
        {
            return await Task.Run(() =>
            {
                try
                {
                    _DBContext.Database.Migrate();
                    return true;
                }
                catch(Exception ex)
                {

                }
                return false;
            });
        }
    }
}
