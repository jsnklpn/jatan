using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JatanWebApp.Models.ViewModels
{
    public class UserSettingsViewModel
    {
        [Display(Name = "User Image")]
        public string UserImageName { get; set; }

        [Display(Name = "User Image Path")]
        public string UserImagePath { get; set; }
    }
}