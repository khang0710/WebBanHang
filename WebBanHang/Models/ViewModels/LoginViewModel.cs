using System;
using System.Collections.Generic;
namespace WebBanHang.Models.ViewModels
{
    public class LoginViewModel
    {
        public int? UserId { get; set; }

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Url { get; set; }
    }
}
