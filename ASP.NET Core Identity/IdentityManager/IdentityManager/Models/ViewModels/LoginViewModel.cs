﻿using System.ComponentModel.DataAnnotations;

namespace IdentityManager.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required, Display(Name = "User name")]
        public string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
