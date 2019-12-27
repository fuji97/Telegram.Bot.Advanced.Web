namespace Telegram.Bot.Advanced.Web.Infrastructure {
    public interface ITelegramWebConfigs {
        string Path { get; }
        string Username { get; }
        string Password { get; }
        bool LoginRequired { get; }
    }
}