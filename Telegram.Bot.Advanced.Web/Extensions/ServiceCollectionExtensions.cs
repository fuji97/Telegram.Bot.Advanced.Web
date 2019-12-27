using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Advanced.Holder;
using Telegram.Bot.Advanced.Web.Infrastructure;

namespace Telegram.Bot.Advanced.Web.Extensions {
    public static class ServiceCollectionExtensions {
        public static IServiceCollection ConfigureTelegramWeb(this IServiceCollection services,
            Action<TelegramWebConfigs> configs = null) {
            var telegramWebConfigs = new TelegramWebConfigs();
            
            configs?.Invoke(telegramWebConfigs);
            services.AddSingleton<ITelegramWebConfigs, TelegramWebConfigs>(fact => telegramWebConfigs);
            
            services.ConfigureOptions(typeof(EditorRCLConfigureOptions));
            
            services.AddAuthentication(options =>
            {
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => { options.LoginPath = "/" + telegramWebConfigs.Path + "/Admin/Login"; });

            return services;
        }
    }
}