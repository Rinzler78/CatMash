using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatMash.Repository
{
    public interface ICatMashRepository
    {
        bool IsLoaded { get; }
        Task Load(bool force = false);
        IReadOnlyList<Cat> Cats { get; }
        int Rate(string winnerId, string opponentId);
    }
}
