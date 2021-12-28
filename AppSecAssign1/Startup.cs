using AppSecAssign1.Models;
using AppSecAssign1.Services;
using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace AppSecAssign1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AppSecDbConnection"));
            });
            services.AddRazorPages();
            services.AddReCaptcha(Configuration.GetSection("ReCaptcha"));
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddHttpContextAccessor();
            services.AddTransient<UserService>();
            services.AddTransient<AppDbContext>();
            services.AddTransient<IMailService, EmailSender>();
            services.AddSingleton<HtmlEncoder>(
                HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.BasicLatin }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();
            app.UseStatusCodePagesWithRedirects("/errors/{0}");

            //app.Use(async (context, next) =>
            //{
            //    string currentSessionUser = context.Session.GetString("username");
            //    // if (context.Session.GetString("username") == null && context.Session.GetString("AuthToken") == null)
            //    // If user is already in Login Page, there's no need to redirect to login page anymore...
            //    if (!context.Request.Path.Value.Contains("/Login"))
            //    {
            //        if (string.IsNullOrEmpty(currentSessionUser))
            //        {
            //            context.Response.Redirect("/Index");
            //            return;
            //        }
            //    }
            //    await next();
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
