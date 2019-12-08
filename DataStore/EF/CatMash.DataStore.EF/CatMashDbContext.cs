using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CatMash.DataStore.EF
{
    public abstract class CatMashDbContext : DbContext
    {
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

                Console.WriteLine($"=> DataStore Entity tracker : {entrie.Entity.GetType().Name} Entity {entrie.State}");

                switch (entrie.State)
                {
                    case EntityState.Added:
                        {
                            entity.CreationDate = now;
                            entity.ModificationDate = now; /** ModificationDate = CreationDate because if we want to now which entity is the last update. We need to compare between CreationDate & Modification. */
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
    }
}
