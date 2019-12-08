using System;
using System.Linq;

namespace CatMash.DataStore.EF
{
    public static partial class CatMashDbContextHelper
    {
        public static Image GetImage(this CatMashDbContext catMashDbContext, string url, bool createOfNotExist = true)
        {
            lock (catMashDbContext)
            {
                var image = catMashDbContext.Images.FirstOrDefault(arg => arg.URL == url);

                if (image == null && createOfNotExist)
                {
                    image = new Image
                    {
                        URL = url
                    };

                    catMashDbContext.Images.Add(image);
                }

                return image;
            }
        }
    }
}
