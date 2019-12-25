#if DEBUG
#define SHORT_LIST
#endif
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using CatMash.DataStore.EF.SQLite;
using CatMash.DataStore.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CatMash.Repository
{
    public class CatMashRepository : TrackerObject, ICatMashRepository
    {
        readonly CatMashDbContext CatMashDbContext;

        public CatMashRepository(CatMashDbContext catMashDbContext)
        {
            CatMashDbContext = catMashDbContext;
        }

        IReadOnlyList<Cat> ICatMashRepository.Cats
        {
            get
            {
                lock (CatMashDbContext)
                {
                    var result = CatMashDbContext.Cats.Include(arg => arg.Image).Select(arg => new Cat
                    {
                        Id = arg.Name,
                        NbMash = (ushort)arg.NbMash,
                        Rate = (ushort)arg.Rate,
                        Url = arg.Image.URL
                    }).ToList();

                    return result;
                }
            }
        }

        public int Rate(string winnerId, string opponentId)
        {
            Debug.WriteLine($"****** => Rate : Winner ({winnerId}) <=> Opponent ({opponentId})");

            CatMash.DataStore.Cat winnerCat, opponentCat;

            lock (CatMashDbContext)
            {
                var cats = CatMashDbContext.Cats.Where(arg => arg.Name == winnerId || arg.Name == opponentId);

                winnerCat = cats.FirstOrDefault(arg => arg.Name == winnerId);
                opponentCat = cats.FirstOrDefault(arg => arg.Name == opponentId);


                if (winnerCat != null && opponentCat != null)
                {
                    ++winnerCat.Rate;

                    ++winnerCat.NbMash;
                    ++opponentCat.NbMash;

                    Debug.WriteLine($"****** => Rate :\n- Winner ({winnerCat})\n- Opponent ({opponentCat})");

                    //CatMashDbContext.SaveChanges();

                    return winnerCat.Rate;
                }
            }

            return -1;
        }
    }
}
