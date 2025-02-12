using Npgsql.EntityFrameworkCore;
using System.Diagnostics;
using apiRestPostgreSql.Models;
using apiRestPostgreSql.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace apiRestPostgreSql.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            
            return View("Index");
        }

    }
}
