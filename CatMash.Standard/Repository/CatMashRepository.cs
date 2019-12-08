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
using CatMash.SQLite;
using Microsoft.EntityFrameworkCore;

namespace CatMash.Repository
{
    public class CatMashRepository : ICatMashRepository
    {
        readonly CatMashDbContext CatMashDbContext = new CatMashSQLiteDbContext();

        public CatMashRepository()
        {
            Load();
        }

        public bool IsLoaded { get; private set; }

        Task LoadTask;
        public Task Load(bool force = false)
        {
            if ((!IsLoaded || force) && (LoadTask?.IsCompleted ?? true))
                LoadTask = LoadTaskRoutine();
            return LoadTask;
        }

        IReadOnlyList<Cat> ICatMashRepository.Cats
        {
            get
            {
                lock (CatMashDbContext)
                {
                    return CatMashDbContext.Cats.Select(arg => new Cat
                    {
                        Id = arg.Name,
                        NbMash = (ushort)arg.NbMash,
                        Rate = (ushort)arg.Rate,
                        Url = arg.Image.URL
                    }).ToList();
                }
            }
        }


        Task LoadTaskRoutine()
        {
            return Task.Run(() =>
            {
                try
                {
                    lock (CatMashDbContext)
                    {
                        CatMashDbContext.Database.Migrate();
                    }

                    string content = Assembly.GetExecutingAssembly().ReadToEnd("cats.json");

                    var dico = JsonConvert.DeserializeObject<Dictionary<string, Cat[]>>(content);

                    var array = dico["images"];


                    var cats =
#if SHORT_LIST
                        array.Take(5);
#else
                        array;
#endif
                    lock (CatMashDbContext)
                    {
                        foreach (var cat in cats)
                        {
                            var existingImage = CatMashDbContext.Images.FirstOrDefault(arg => arg.URL == cat.Url);

                            if (existingImage == null)
                                CatMashDbContext.Images.Add(new SQLite.Image
                                {
                                    URL = cat.Url
                                });

                            var existingCat = CatMashDbContext.Cats.FirstOrDefault(arg => arg.Name == cat.Id);

                            if (existingCat == null)
                                CatMashDbContext.Cats.Add(new SQLite.Cat
                                {
                                    Image = existingImage,
                                    Name = cat.Id
                                });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        public int Rate(string winnerId, string opponentId)
        {
            CatMash.SQLite.Cat winnerCat, againstCat;

            lock (CatMashDbContext)
            {
                var cats = CatMashDbContext.Cats.Where(arg => arg.Name == winnerId || arg.Name == opponentId);

                winnerCat = cats.FirstOrDefault(arg => arg.Name == winnerId);
                againstCat = cats.FirstOrDefault(arg => arg.Name == opponentId);
            }

            if (winnerCat != null && againstCat != null)
            {
                ++winnerCat.Rate;

                ++winnerCat.NbMash;
                ++againstCat.NbMash;

                return winnerCat.Rate;
            }

            return -1;
        }

        public void ClearVotes()
        {
            lock (CatMashDbContext)
            {
                var mashs = CatMashDbContext.Mash.ToList();

                mashs.ForEach(arg =>
                {
                    arg.LeftCat.NbMash = 0;
                    arg.LeftCat.Rate = 0;

                    arg.RightCat.NbMash = 0;
                    arg.RightCat.Rate = 0;
                });

                CatMashDbContext.Mash.RemoveRange(mashs);
            }
        }
    }
}
