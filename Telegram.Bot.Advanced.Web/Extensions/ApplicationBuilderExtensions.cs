using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Advanced.Exceptions;
using Telegram.Bot.Advanced.Extensions;
using Telegram.Bot.Advanced.Holder;
using Telegram.Bot.Advanced.Middlewares;
using Telegram.Bot.Advanced.Web.Infrastructure;

namespace Telegram.Bot.Advanced.Web.Extensions {
    public static class ApplicationBuilderExtensions {
        public static IApplicationBuilder UseTelegramEndpoints(this IApplicationBuilder app, Action<IEndpointRouteBuilder> endpoints = null) {
            var holder = app.ApplicationServices.GetService<ITelegramHolder>();
            if (holder == null) {
                throw new TelegramHolderNotInjectedException();
            }

            ITelegramWebConfigs telegramWebConfigs = app.ApplicationServices.GetService<ITelegramWebConfigs>();
            if (telegramWebConfigs == null) {
                telegramWebConfigs = new TelegramWebConfigs();
            }
            
            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(e => {
                e.MapControllerRoute("telegramweb", telegramWebConfigs.Path + "/{controller=Admin}/{action=Index}/{key?}");
                endpoints?.Invoke(e);
            });
            return app;
        }
    }
}