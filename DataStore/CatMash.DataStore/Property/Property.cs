using System;
namespace CatMash.DataStore
{
    public class Property : IdObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
