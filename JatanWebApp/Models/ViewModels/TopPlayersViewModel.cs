using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JatanWebApp.Models.ViewModels
{
    public class TopPlayersViewModel
    {
        public PagedList<ApplicationUserViewModel> UserList { get; set; }

        public TopPlayersViewModel(PagedList<ApplicationUserViewModel> userList)
        {
            this.UserList = userList;
        }
    }
}