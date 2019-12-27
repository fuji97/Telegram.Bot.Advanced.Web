using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Advanced.Holder;

namespace Telegram.Bot.Advanced.Web.Models {
    public class DefaultViewModel {
        public List<string> Bots { get; set; } = new List<string>();
        
        public DefaultViewModel() {}

        public DefaultViewModel(ITelegramHolder holder) {
            Bots = holder.Select(b => b.Username).ToList();
        }
    }
}