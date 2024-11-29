using CMS.DAL.Data;
using CMS.DAL.Entities;
using CMS.UI.Services;
using CMS.UI.Services.Interfaces;
using CMS.UI.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;

namespace CMS.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddMvc().AddRazorRuntimeCompilation().AddNToastNotifyToastr();
     
            builder.Services.AddScoped<IEmailService,EmailService>();
            builder.Services.AddValidatorsFromAssemblyContaining<BranchValidator>();

            builder.Services.AddDbContext<CMSDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlConStr"));
            });

            builder.Services.AddIdentity<AppUser, AppRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                opt.SignIn.RequireConfirmedEmail = true;


            }).AddEntityFrameworkStores<CMSDbContext>().AddDefaultTokenProviders();


            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
          // app.UseSerilogRequestLogging();
          //  app.UseHttpLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            /*app.UseAuthentication();*/
            app.UseAuthorization();
            //app.Use(async (context, next) =>
            //{
            //    var userName = context.User?.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null;
            //    LogContext.PushProperty("user_name", userName);
            //    await next();
            //});
           app.UseNToastNotify();
            app.MapControllerRoute(
                      name: "areas",
                      pattern: "{area:exists}/{controller=Account}/{action=LogIn}/{id?}"
                    );
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=LogIn}/{id?}");
           // app.UseCors();
          
            app.Run();
        }
    }
}
