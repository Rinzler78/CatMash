using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CatMash.Controllers
{
    public class HomeController : Controller
    {
        readonly ICatMashRepository CatsRepository;
        public HomeController(ICatMashRepository catsRepository)
        {
            CatsRepository = catsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IReadOnlyList<Cat>> AllCats()
        {
            await CatsRepository.Load();
            return CatsRepository.Cats;
        }

        [HttpPost]
        public bool Vote(string id)
        {
            return false;
        }
    }
}
