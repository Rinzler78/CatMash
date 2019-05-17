using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
                        CatsList.AddRange(array);
                    }
                }
                catch (Exception ex)
                {

                }
            });
        }
    }
}
