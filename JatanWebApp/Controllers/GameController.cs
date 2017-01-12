using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JatanWebApp.Models.ViewModels;

namespace JatanWebApp.Controllers
{
    [Authorize]
    public class GameController : BaseController
    {
        // GET: Game
        public ActionResult Index()
        {
            return View();
        }

        // Get: Game/Create
        public ActionResult Create()
        {
            return View(new CreateGameViewModel());
        }

        // Post: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateGameViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        // Get: Game/Join
        public ActionResult Join()
        {
            return View(new JoinGameViewModel());
        }

    }
}