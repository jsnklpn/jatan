using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JatanWebApp.Models;
using JatanWebApp.Models.DAL;
using JatanWebApp.Models.ViewModels;

namespace JatanWebApp.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        // Get: /Home/
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }

        // GET: /Home/TopPlayers?page=1&size=25
        public Microsoft.AspNetCore.Mvc.ActionResult TopPlayers(int? page, int? size)
        {
            int pageIndex = page ?? 1;
            int pageSize = size ?? 25;

            var userVmList = new List<ApplicationUserViewModel>();
            using (var db = new JatanDbContext())
            {
                var users = db.Users.Include("UserImage").ToList();
                foreach (var user in users)
                {
                    userVmList.Add(new ApplicationUserViewModel(user));
                }
            }

            var pagedList = new PagedList<ApplicationUserViewModel>(userVmList.OrderByDescending(u => u.PlayerScore).AsQueryable(), pageIndex, pageSize);
            var vm = new TopPlayersViewModel(pagedList);
            return View(vm);
        }
    }
}