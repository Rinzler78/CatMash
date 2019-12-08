using System;
using System.Collections.Generic;

namespace CatMash.DataStore
{
    public class Cat : NameObject
    {
        public Image Image { get; set; }
        public int NbMash { get; set; }
        public int Rate { get; set; }

        public override string ToString()
        {
            return $"{base.ToString()}, Image : ({Image}), NbMash : ({NbMash}), Rate : ({Rate}) ";
        }
    }
}
