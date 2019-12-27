using System.ComponentModel.DataAnnotations;

namespace Telegram.Bot.Advanced.Web.Models {
    public class LoginModel : DefaultViewModel {
        public LoginForm FormData { get; set; }

        public class LoginForm {
            [Required]
            public string Username { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }
    }
}