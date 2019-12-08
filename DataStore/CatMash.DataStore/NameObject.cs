using System;
using System.ComponentModel.DataAnnotations;

namespace CatMash.DataStore
{
    public class NameObject : DBObject
    {
        [Key]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Name : {Name}";
        }
    }
}
