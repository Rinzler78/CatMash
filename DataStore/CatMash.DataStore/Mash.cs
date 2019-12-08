using System;
using System.Collections.Generic;

namespace CatMash.DataStore
{
    public class Mash : IdObject
    {
        public Cat LeftCat { get; set; }
        public Cat RightCat { get; set; }

        public Cat WinnerCat { get; set; }
    }
}
