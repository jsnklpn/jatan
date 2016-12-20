using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JatanWebApp.Controllers
{
    public class GameController : BaseController
    {
        // GET: Game
        public ActionResult Index()
        {
            return View();
        }
    }
}