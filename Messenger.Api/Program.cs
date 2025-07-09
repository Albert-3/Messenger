using FluentValidation;
using Messaner.Infrastructure.Repositories;
using Messegner.Infrastructure.Repositories;
using Messenger.App.Authentication;
using Messenger.App.DTOs;
using Messenger.App.Hubs;
using Messenger.App.Services;
using Messenger.App.Validators;
using Messenger.Domain.Interface;
using Messenger.Domain.Interfaces;
using Messenger.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Messenger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddControllersWithViews();
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddSignalR();

            // DB Context
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Messenger.Infrastructure")));

            // Dependency Injection
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IValidator<RegistrationDTO>, RegistrationValidator>();
            builder.Services.AddScoped<IValidator<LoginDTO>, LoginValidator>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IGenerateChatRoomName, ChatHub>();

            builder.Services.AddScoped<MessageService>();
            // JWT Configuration from appsettings.json
            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
            // Configuration binding

            var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

            //  Authentication (Only one AddAuthentication!)
            builder.Services.AddAuthentication(options =>
            {
                // Set default scheme to Cookies for browser requests
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                // For API requests, we'll use JWT Bearer
                options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddCookie(options =>
             {
                 options.LoginPath = "/User/Login";
                 options.Cookie.Name = "messenger-auth";
                 options.ExpireTimeSpan = TimeSpan.FromDays(30); // Example: 30-day persistent cookie
                 options.SlidingExpiration = true;
             })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key)
                 };
             });


            // Build App
            var app = builder.Build();
            app.MapHub<ChatHub>("/chatHub");

            // Middleware
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();

            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
