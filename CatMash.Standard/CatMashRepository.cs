﻿#if DEBUG
#define SHORT_LIST
#endif
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace CatMash
{
    public class CatMashRepository : ICatMashRepository
    {
        readonly List<Cat> CatsList = new List<Cat>();

        public CatMashRepository()
        {
            Load();
        }

        public bool IsLoaded => CatsList.Count > 0;

        Task LoadTask;
        public Task Load(bool force = false)
        {
            if ((!IsLoaded || force) && (LoadTask?.IsCompleted ?? true))
                LoadTask = LoadTaskRoutine();
            return LoadTask;
        }

        const string LAtelierCatsUrl = "https://latelier.co/data/cats.json";

        IReadOnlyList<Cat> ICatMashRepository.Cats => CatsList;

        Task LoadTaskRoutine()
        {
            return Task.Run(async () =>
            {
                try
                {
                    var httpClient = new HttpClient();
                    var response = await httpClient.GetAsync(LAtelierCatsUrl);

                    //will throw an exception if not successful
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();

                    var dico = JsonConvert.DeserializeObject<Dictionary<string, Cat[]>>(content);

                    var array = dico["images"];

                    lock (CatsList)
                    {
                        CatsList.Clear();

                        var cats =
#if SHORT_LIST
                        array.Take(5);
#else
                        array;
#endif

                        CatsList.AddRange(cats);
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }

        public int Rate(string winnerId, string opponentId)
        {
            Cat winnerCat, againstCat;

            lock (CatsList)
            {
                winnerCat = CatsList.FirstOrDefault(arg => arg.Id == winnerId);
                againstCat = CatsList.FirstOrDefault(arg => arg.Id == opponentId);
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
            lock (CatsList)
            {
                CatsList.ForEach(arg =>
                {
                    arg.Rate = 0;
                    arg.NbMash = 0;
                });
            }
        }
    }
}
