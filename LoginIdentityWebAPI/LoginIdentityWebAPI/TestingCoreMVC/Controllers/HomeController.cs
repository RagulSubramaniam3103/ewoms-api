using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using TestingCoreMVC.Models;

namespace TestingCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(int Page = 1)
        {
            var getmoredata = Enumerable.Range(1, 1000).Select(x => new UserViewModel
            {
                Id=x,
                Name = "Test"+x,
                Email = "Email"+x+"@gmail.com",
                Phone = Random.Shared.Next(1000000000, 1999999999)
            });
            int gettotalrecords = getmoredata.Count();
            int PageSize = 10;

            ViewBag.Currentpage = Page;
            ViewBag.TotalPages = (int)Math.Ceiling(getmoredata.Count() / (double)PageSize);

            var setpage = getmoredata.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

            return View(setpage);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
