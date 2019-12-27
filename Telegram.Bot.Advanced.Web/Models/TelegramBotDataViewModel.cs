using Telegram.Bot.Advanced.Holder;

namespace Telegram.Bot.Advanced.Web.Models {
    public class TelegramBotDataViewModel : DefaultViewModel {
        public TelegramBotDataViewModel(ITelegramHolder holder) : base(holder) {
        }

        public ITelegramBotData Bot { get; set; }
        public string WebhookUrl { get; set; }
    }
}