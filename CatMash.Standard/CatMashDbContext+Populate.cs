using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CatMash.DataStore.EF;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CatMash
{
    public static class CatMashDbContextExt
    {
        public static bool Populate(this CatMashDbContext catMashDbContext)
        {
            try
            {
                string content = Assembly.GetExecutingAssembly().ReadToEnd("cats.json");

                var dico = JsonConvert.DeserializeObject<Dictionary<string, Cat[]>>(content);

                var array = dico["images"];

                var cats = array;

                lock (catMashDbContext)
                {
                    foreach (var cat in cats)
                    {
                        var dbImage = catMashDbContext.GetImage(cat.Url);
                        var dbCat = catMashDbContext.GetCat(cat.Id);

                        dbCat.Image = dbImage;
                    }

                    catMashDbContext.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"****** => Populate exception : {ex}");
            }
            return false;
        }
    }
}
