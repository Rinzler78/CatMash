using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CatMash.DataStore.EF
{
    public abstract partial class CatMashDbContext : DbContext
    {
        public DbSet<Property> Properties { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Mash> Mash { get; set; }

        protected CatMashDbContext()
        {
        }

        protected CatMashDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public override int SaveChanges()
        {
            ProcessSaveChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ProcessSaveChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        void ProcessSaveChanges()
        {
            DateTime now = DateTime.UtcNow;

            var query = ChangeTracker.Entries().Where(p => (p.State == EntityState.Modified) || (p.State == EntityState.Added));

            foreach (var entrie in query)
            {
                if (!(entrie.Entity is DBObject entity))
                    continue;

                Debug.WriteLine($"=> DataStore Entity tracker : {entrie.Entity.GetType().Name} Entity {entrie.State}");

                switch (entrie.State)
                {
                    case EntityState.Added:
                        {
                            entity.CreationDate = now;
                            entity.ModificationDate = now;
                        }
                        break;

                    case EntityState.Modified:
                        {
                            entity.ModificationDate = now;
                        }
                        break;
                }
            }
        }

        Task<bool> _InitTask;
        public Task<bool> Init()
        {
            lock (this)
            {
                if (_InitTask?.IsCompleted ?? true)
                    _InitTask = InitTaskRoutine();

                return _InitTask;
            }
        }

        public bool IsInitialized { get; private set; }

        async Task<bool> InitTaskRoutine()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (!IsInitialized)
                    {
                        var startDate = DateTime.UtcNow;

                        Debug.WriteLine($"Init {GetType().Name} : Begin");

                        var result = InternalInit();

                        Debug.WriteLine($"Init {GetType().Name} : End, duration ({(DateTime.UtcNow - startDate).ToString("mm':'ss':'fff")})");

                        IsInitialized = true;

                        return result;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return false;
            }).ConfigureAwait(false);
        }

        const string LastInitPropertyName = "Last Init";

        protected virtual bool InternalInit()
        {
            bool result = false;
            var startDate = DateTime.UtcNow;

            try
            {
                string currentMigrationId;

                this.GetProperty(nameof(MigrationId), out currentMigrationId);

                var needMigration = currentMigrationId != MigrationId;

                if (needMigration)
                {
                    result = Migrate() && this.SetProperty(nameof(MigrationId), MigrationId) && SaveChanges() > 0;
                }
                else
                {
                    Debug.WriteLine($"Init {GetType().Name} : no migration needed");
                    result = true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Init {GetType().Name} : Exception {e}");
            }

            return result;
        }

        const byte TryMigrateMaxCount = 1;
        byte TryMigrateCount;

        public bool Migrate()
        {
            try
            {
                Debug.WriteLine($"Init {GetType().Name} : Try Migrate");

                Database.Migrate();

                Debug.WriteLine($"Init {GetType().Name} : Migrate Success");

                return true;
            }
            catch (Exception e)
            {
                /** NEVER Remove this condition, otherwise, the main sql server data business will be ERASED/REMOVED/DESTROYED */
                if (GetType().Name == "SQLiteGoodAngelDbContext")
                {
                    Debug.WriteLine($"Init {GetType().Name} : Migrate failed {e} => try to delete");

                    var deleted = Database.EnsureDeleted();

                    if (!deleted)
                        Debug.WriteLine($"Init {GetType().Name} : EnsureDeleted failed");

                    if (TryMigrateCount < TryMigrateMaxCount)
                    {
                        ++TryMigrateCount;
                        InternalInit();
                    }

                    TryMigrateCount = 0;
                }
                else
                {
                    Debug.WriteLine($"Init {GetType().Name} : Migrate failed {e} ");
                }
            }
            return false;
        }
    }
}
