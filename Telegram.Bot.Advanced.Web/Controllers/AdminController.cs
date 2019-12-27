using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Advanced.DbContexts;
using Telegram.Bot.Advanced.Holder;
using Telegram.Bot.Advanced.Web.Infrastructure;
using Telegram.Bot.Advanced.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Telegram.Bot.Advanced.Web.Controllers {
    [Authorize]
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller {
        private readonly ITelegramHolder _holder;
        private readonly ITelegramWebConfigs _configuration;

        public AdminController(ITelegramHolder holder, ITelegramWebConfigs configuration) {
            _holder = holder;
            _configuration = configuration;
        }
        
        //[HttpGet("")]
        public async Task<IActionResult> Index() {
            return View(new DefaultViewModel(_holder));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel.LoginForm formData, bool foo = false) {
            LoginModel loginModel = new LoginModel {FormData = formData};

            if (_configuration.LoginRequired == false) {
                await Login("", "", false);
                return RedirectToAction("Index");
            }
            
            if (ModelState.IsValid) {
                var isValid = (formData.Username == _configuration.Username && formData.Password == _configuration.Password);
                if (!isValid)
                {
                    ModelState.AddModelError("", "username o password invalidi");
                    return View(loginModel);
                }

                await Login(formData.Username, formData.Password, formData.RememberMe);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "username o password vuoti");
                return View(loginModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel) {
            if (_configuration.LoginRequired == false) {
                await Login("", "", false);
                return RedirectToAction("Index");
            }
            
            return View();
        }

        //[HttpGet("{key}")]
        public async Task<IActionResult> Bot(string key) {
            ITelegramBotData bot = GetBot(key);

            if (bot == null) {
                return RedirectToAction("Index");
            }
            return View(GetBotDataViewModel(_holder, bot));
        }

        public async Task<IActionResult> SetWebhook(string key) {
            ITelegramBotData bot = GetBot(key);

            if (bot == null) {
                return Ok(new {status = "error", description = "Nessun bot specificato"});
            }

            var webhookUrl = GetWebhookString(bot);
            try {
                await bot.Bot.SetWebhookAsync(webhookUrl);
            }
            catch (Exception e) {
                return Ok(new {status = "error", description = e.Message});
            }
            return Ok(new {status = "ok", webhook = webhookUrl});
        }
        
        public async Task<IActionResult> DeleteWebhook(string key) {
            ITelegramBotData bot = GetBot(key);

            if (bot == null) {
                return Ok(new {status = "error", description = "Nessun bot specificato"});
            }
            
            try {
                await bot.Bot.DeleteWebhookAsync();
            }
            catch (Exception e) {
                return Ok(new {status = "error", description = e.Message});
            }
            return Ok(new {status = "ok"});
        }

        public async Task<IActionResult> Migrate([FromServices] TelegramContext context) {
            try {
                await context.Database.MigrateAsync();
            }
            catch (Exception e) {
                return Ok(new {status = "error", description = e.Message});
            }

            return Ok(new {status = "ok"});
        }

        private async Task Login(string username, string password, bool rememberMe) {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            identity.AddClaim(new Claim(ClaimTypes.Name, username));
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = rememberMe });
        }

        private ITelegramBotData GetBot(string username) {
            return _holder.FirstOrDefault(b => b.Username == username);
        }

        private TelegramBotDataViewModel GetBotDataViewModel(ITelegramHolder holder, ITelegramBotData bot) {
            return new TelegramBotDataViewModel(holder) {
                Bot = bot,
                WebhookUrl = GetWebhookString(bot)
            };
        }

        private string GetWebhookString(ITelegramBotData bot) {
            return $"{Request.Scheme}://{Request.Host}{bot.BasePath}/{bot.Endpoint}";
        }
    }
}