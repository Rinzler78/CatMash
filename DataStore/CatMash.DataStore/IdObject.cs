using System;
namespace CatMash.DataStore
{
    public abstract class IdObject : DBObject
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return $"Id : ({Id})";
        }
    }
}
