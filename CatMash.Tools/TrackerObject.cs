using System;
using System.Diagnostics;

namespace CatMash
{
    public class TrackerObject
    {
        public TrackerObject()
        {
            Debug.WriteLine($"****** => Creating {GetType().Name}");
        }

        ~TrackerObject()
        {
            Debug.WriteLine($"****** => Destroying {GetType().Name}");
        }
    }
}
