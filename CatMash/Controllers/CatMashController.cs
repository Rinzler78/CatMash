using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CatMash.ClientManager;
using CatMash.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatMash.Controllers
{
    public class CatMashController : Controller
    {
        readonly ICatMashRepository CatsRepository;
        readonly ICatMashClientManager CatMashClientManager;
        public CatMashController(ICatMashRepository catsRepository, ICatMashClientManager catMashClientManager)
        {
            CatsRepository = catsRepository;
            CatMashClientManager = catMashClientManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }

        public async Task<IReadOnlyList<Cat>> AllCats()
        {
            await CatsRepository.Load();
            return CatsRepository.Cats;
        }

        public int Rate(string winnerId, string opponentId)
        {
            if (string.IsNullOrEmpty(winnerId) || string.IsNullOrEmpty(opponentId))
                return -1;

            var result = CatsRepository.Rate(winnerId, opponentId);

            CatMashClientManager.NotifyRate(winnerId, opponentId);

            return result;
        }
    }
}
