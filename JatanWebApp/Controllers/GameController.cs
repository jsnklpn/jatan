using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JatanWebApp.Helpers;
using JatanWebApp.Models.ViewModels;
using JatanWebApp.SignalR;
using Microsoft.AspNet.Identity;

namespace JatanWebApp.Controllers
{
    [Authorize]
    public class GameController : BaseController
    {
        // GET: Game
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return RedirectToAction("Join");
        }

        // Get: Game/Create
        public Microsoft.AspNetCore.Mvc.ActionResult Create()
        {
            var userName = User.Identity.Name;
            var defaultGameName = string.Format("{0}'s game", userName);

            return View(new CreateGameViewModel() { DisplayName = defaultGameName });
        }

        // Post: Game/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Microsoft.AspNetCore.Mvc.ActionResult Create(CreateGameViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var userName = User.Identity.Name;
            var avatarPath = User.Identity.GetAvatarPath();
            var lobby = GameLobbyManager.CreateNewGame(userName, avatarPath, viewModel);

            return RedirectToAction("Instance", new { gameId = lobby.Uid });
        }

        // Get: Game/Join/?gameId={id}&password={password}
        public Microsoft.AspNetCore.Mvc.ActionResult Join(string gameId, string password)
        {
            if (!string.IsNullOrEmpty(gameId))
            {
                var lobby = GameLobbyManager.GetGameLobbyFromUid(gameId);
                if (lobby != null)
                {
                    var userName = User.Identity.Name;
                    var imagePath = User.Identity.GetAvatarPath();
                    var result = GameLobbyManager.ConnectToGame(userName, lobby.Owner, password, imagePath);
                    if (result.Succeeded)
                        return RedirectToAction("Instance", new {gameId = gameId});

                    return View(new JoinGameViewModel() {ErrorMessage = result.Message});
                }
                return View(new JoinGameViewModel() { ErrorMessage = "This game does not exist." });
            }

            return View(new JoinGameViewModel());
        }

        // Get: Game/Instance/?gameId={id}
        public Microsoft.AspNetCore.Mvc.ActionResult Instance(string gameId)
        {
            if (!string.IsNullOrEmpty(gameId))
            {
                var lobby = GameLobbyManager.GetGameLobbyFromUid(gameId);
                if (lobby != null)
                {
                    var userName = User.Identity.Name;
                    if (lobby.Players.Contains(userName))
                        return View(lobby);

                    return View("Join", new JoinGameViewModel() { ErrorMessage = "Unauthorized." });
                }
            }
            return View("Join", new JoinGameViewModel() { ErrorMessage = "Game not found." });
        }

        // Get: Game/PostGame/?gameId={id}
        public Microsoft.AspNetCore.Mvc.ActionResult PostGame(string gameId)
        {
            if (gameId == "test")
                return View(PostGameViewModel.CreateTestVm());

            var vm = new PostGameViewModel(gameId);
            if (!string.IsNullOrEmpty(vm.ErrorMessage))
                return RedirectToAction("Index", "Home");
            return View(new PostGameViewModel(gameId));
        }

    }
}