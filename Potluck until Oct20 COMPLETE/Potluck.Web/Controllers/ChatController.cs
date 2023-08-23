using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Potluck.Web.Controllers
{
    public class ChatController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("InsertUserName");
        }

        [HttpPost]
        public IActionResult Index(string username)
        {
            return View("Index", username);
        }
    }
}