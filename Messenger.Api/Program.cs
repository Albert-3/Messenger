using FluentValidation;
using Messaner.Infrastructure.Repositories;
using Messegner.Infrastructure.Repositories;
using Messenger.App.DTOs;
using Messenger.App.Services;
using Messenger.App.Validators;
using Messenger.Domain.Interface;
using Messenger.Domain.Interfaces;
using Messenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Messenger.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddMvc();
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Messenger.Infrastructure")));

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IValidator<RegistrationDTO>, RegistrationValidator>();
            builder.Services.AddScoped<IValidator<LoginDTO>, LoginValidator>();
            builder.Services.AddScoped<MessageService>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();



            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseSession();
            app.MapControllers();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=User}/{action=Login}/{id?}");

            app.Run();

        }
    }
}
