using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using _1.AuthorizationRequirement;
using _1.Data;
using _1.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _1
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Memory"));

            services.AddIdentity<PlantsistEmployee, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config => {
                config.Cookie.Name = "Grandma.Cookie";
                config.LoginPath = "/Home/Login";
            });

            services.AddScoped<IUserClaimsPrincipalFactory<PlantsistEmployee>,
                PlantsistEmployeeClaimsprincipalFactory>();

            services.AddAuthorization(options => {
                options.AddPolicy("Claim.Level", builder => {
                    builder.AddRequirements(new CustomClaimRequirement("level"));
                });

                options.AddPolicy("Technology", builder =>
                {
                    builder.AddRequirements(new DepartmentRequirement("Technology"));
                });
            });

            services.AddScoped<IAuthorizationHandler, CustomClaimRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, DepartmentrequirementHandler>();

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
