using dsd03Ass2MVC.Data;
using dsd03Ass2MVC.DTO;
using dsd03Ass2MVC.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Diagnostics;

namespace dsd03Ass2MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var allSales = _context.Order.Include(o => o.Stock).OrderByDescending(o => o.Stock.Price).Take(15);


            List<TopSales> topStock = new List<TopSales>();

            foreach (var item in allSales)
            {
                TopSales sales = new TopSales();
                sales.ProductName = item.Stock.ProductName;
                sales.ProductDescription = item.Stock.ProductDescription;
                sales.ProductType = item.Stock.ProductType;
                sales.Price = item.Stock.Price;
                topStock.Add(sales);
            }


            List<MostSold> mostSold = new List<MostSold>();

            mostSold.AddRange((IEnumerable<MostSold>)topStock.GroupBy(n => n.ProductName)
                                     .Select(n => new MostSold
                                     {
                                         ProductName = n.Key,
                                         Count = n.Count()
                                     })
                                     .OrderByDescending(n => n.Count)
                                     );

            ViewData["MostSold"] = mostSold.Take(5);

            // ViewData["TopSales"] = topStock;
            return View(topStock);
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