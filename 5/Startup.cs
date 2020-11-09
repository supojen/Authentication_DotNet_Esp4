using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _5.AuthorizationRequirements;
using _5.Data;
using _5.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace _5
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options => {
                options.UseInMemoryDatabase("Memory");
            });

            services.AddIdentity<PlantsistEmployee, IdentityRole<Guid>>(opts => { })
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opts => {
                opts.Cookie.Name = "Plantsist.Cookie";
                opts.LoginPath = "/Home/Login";
            });

            services.AddScoped<IUserClaimsPrincipalFactory<PlantsistEmployee>, PlantsistEmployeeUserClaimsPrincipalFactory>();

            services.AddSingleton<IAuthorizationPolicyProvider, CompanyAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, DepartmentRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, LevelRequirementHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
