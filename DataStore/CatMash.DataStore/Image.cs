using System;
namespace CatMash.DataStore
{
    public class Image : IdObject
    {
        public string URL { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, Url : ({URL})";
        }
    }
}
