using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VS_portfolio_2026.Models;

namespace VS_portfolio_2026.Controllers
{
    public class PortfolioController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chromaic()
        {
            return View();
        }
    }
}

