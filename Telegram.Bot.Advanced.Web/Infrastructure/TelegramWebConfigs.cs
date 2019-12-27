using System.Diagnostics.CodeAnalysis;

namespace Telegram.Bot.Advanced.Web.Infrastructure {
    public class TelegramWebConfigs : ITelegramWebConfigs {
        public string Path { get; set; } = "web";
        public string Username { get; set; } = "telegramweb";
        public string Password { get; set; } = "telegramweb";
        public bool LoginRequired { get; set; } = false;

        public void SetLogin([NotNull] string username, [NotNull] string password) {
            LoginRequired = true;
            Username = username;
            Password = password;
        }
    }
}