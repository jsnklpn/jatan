using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JatanWebApp.Models.ViewModels;
using JatanWebApp.SignalR;
using Microsoft.AspNet.Identity;

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

            var userName = User.Identity.Name;
            GameLobbyManager.CreateNewGame(userName, viewModel);

            // TODO: Join the game

            return RedirectToAction("Index");
        }

        // Get: Game/Join
        public ActionResult Join(string gameId, string password)
        {
            if (!string.IsNullOrEmpty(gameId))
            {
                var lobby = GameLobbyManager.GetGameLobbyFromUid(gameId);
                if (lobby != null)
                {
                    var userName = User.Identity.Name;
                    var result = GameLobbyManager.ConnectToGame(userName, lobby.Owner, password);
                    if (result.Succeeded)
                        return RedirectToAction("Index");

                    return View(new JoinGameViewModel() {ErrorMessage = result.Message});
                }
                return View(new JoinGameViewModel() { ErrorMessage = "This game does not exist." });
            }

            return View(new JoinGameViewModel());
        }

    }
}