using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Advanced.DbContexts;
using Telegram.Bot.Advanced.Dispatcher;
using Telegram.Bot.Advanced.Extensions;
using Telegram.Bot.Advanced.Holder;
using Telegram.Bot.Advanced.Web.Extensions;
using TestServer.Context;

namespace TestServer {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddEntityFrameworkInMemoryDatabase();
            services.AddDbContext<TestTelegramContext>();
            
            services.AddTelegramHolder(new TelegramBotDataBuilder()
                .UseDispatcherBuilder(new DispatcherBuilder<TestTelegramContext, TelegramController.TelegramController>())
                .CreateTelegramBotClient(Configuration["BotKey"])
                .SetBasePath(Configuration["BasePath"])
                .Build()
            );

            services.AddControllersWithViews();
            services.ConfigureTelegramWeb(confs => {
                confs.SetLogin("fuji", "fuji");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseTelegramPolling();
            app.UseRouting();

            app.UseAuthorization();

            app.UseTelegramEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}