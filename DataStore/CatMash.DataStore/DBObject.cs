using System;
namespace CatMash.DataStore
{
    public abstract class DBObject
    {
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
