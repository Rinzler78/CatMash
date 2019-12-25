using CatMash.Controllers;
using CatMash.Repository;
using CatMash.ClientManager;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using CatMash.DataStore.EF.SQLite;
using CatMash.DataStore.EF;
using System.Diagnostics;

namespace CatMash.Test
{
    public class CatMashControllerTest
    {
        readonly CatMashDbContext CatMashDbContext = new CatMashSQLiteDbContext();
        readonly ICatMashRepository CatsRepository;
        readonly ICatMashClientManager CatMashClientManager = new CatMashClientManager();
        readonly CatMashController CatMashController;

        public CatMashControllerTest()
        {
            CatsRepository = new CatMashRepository(CatMashDbContext);
            CatMashController = new CatMashController(CatsRepository, CatMashClientManager);
        }

        [SetUp]
        public async Task Setup()
        {
            if(await CatMashDbContext.Init())
            {
                CatMashDbContext.Populate();
            }
        }

        [Test]
        public void AllCatsTest()
        {
            (string, string, int)[] testPlan =
            {
                (null, null, -1),
                (null, "", -1),
                ("", null, -1),
                ("", "", -1),
                ("FakeId", "FakeId",  -1)
            };

            var testPlanList = testPlan.ToList();

            try
            {
                var cats = CatsRepository.Cats;

                for (int leftCatIndex = 0; leftCatIndex < cats.Count; ++leftCatIndex)
                {
                    for (int rightCatIndex = 0; rightCatIndex < cats.Count; ++rightCatIndex)
                    {
                        var leftCat = cats[leftCatIndex];

                        if (rightCatIndex == leftCatIndex)
                            ++rightCatIndex;

                        if (rightCatIndex >= cats.Count)
                            continue;

                        var rightCat = cats[rightCatIndex];

                        testPlanList.Add((leftCat.Id, rightCat.Id, int.MinValue));
                    }
                }

                UInt32 testNumber = 0;
                foreach (var test in testPlanList)
                {
                    Debug.WriteLine($"****** => *********** {testNumber} ***********");

                    var result = CatMashController.Rate(test.Item1, test.Item2);

                    if (test.Item3 == int.MinValue)
                        Assert.GreaterOrEqual(result, 0);
                    else
                        Assert.AreEqual(test.Item3, result);

                    Debug.WriteLine($"****** => *********** {testNumber++} ***********");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }
    }
}