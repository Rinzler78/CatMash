using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CatMash.DataStore.EF
{
    public static partial class CatMashDbContextHelper
    {
        public static Cat GetCat(this CatMashDbContext catMashDbContext, string name, bool createOfNotExist = true)
        {
            lock(catMashDbContext)
            {
                var cat = catMashDbContext.Cats.Include(arg => arg.Image).FirstOrDefault(arg => arg.Name == name);

                if (cat == null && createOfNotExist)
                {
                    cat = new Cat
                    {
                        Name = name
                    };

                    catMashDbContext.Cats.Add(cat);
                }

                return cat;
            }
        }
    }
}
