using System;
using System.IO;
using System.Reflection;

namespace CatMash
{
    public static class RessourceHelper
    {
        public static string ReadToEnd(this Assembly assembly, string resourceName)
        {
            var fullResourceName = typeof(Cat).Namespace + "." + resourceName;

            using (Stream stream = assembly.GetManifestResourceStream(fullResourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
