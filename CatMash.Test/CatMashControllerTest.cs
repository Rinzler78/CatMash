using CatMash.Controllers;
using CatMash.Repository;
using CatMash.ClientManager;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CatMash.Test
{
    public class CatMashControllerTest
    {
        readonly ICatMashRepository CatsRepository = new CatMashRepository();
        readonly ICatMashClientManager CatMashClientManager = new CatMashClientManager();
        readonly CatMashController CatMashController;

        public CatMashControllerTest()
        {
            CatMashController = new CatMashController(CatsRepository, CatMashClientManager);
        }

        [SetUp]
        public async Task Setup()
        {
            await CatsRepository.Load();
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
                for (int leftCatIndex = 0; leftCatIndex < CatsRepository.Cats.Count; ++leftCatIndex)
                {
                    for (int rightCatIndex = 0; rightCatIndex < CatsRepository.Cats.Count; ++rightCatIndex)
                    {
                        var leftCat = CatsRepository.Cats[leftCatIndex];

                        if (rightCatIndex == leftCatIndex)
                            ++rightCatIndex;

                        if (rightCatIndex >= CatsRepository.Cats.Count)
                            continue;

                        var rightCat = CatsRepository.Cats[rightCatIndex];

                        testPlanList.Add((leftCat.Id, rightCat.Id, int.MinValue));
                    }
                }

                foreach (var test in testPlanList)
                {
                    var result = CatMashController.Rate(test.Item1, test.Item2);

                    if (test.Item3 == int.MinValue)
                        Assert.GreaterOrEqual(result, 0);
                    else
                        Assert.AreEqual(test.Item3, result);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }
    }
}