﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductExperiences.Data;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Mocks;
using ProductExperiences.Data.Models;
using ProductExperiences.Data.Repositories;
using ProductExperiences.Entities;
using ProductExperiences.Services;
using ReflectionIT.Mvc.Paging;

namespace ProductExperiences
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_config.GetConnectionString("ProductExperiencesDBConnection")));


            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;

            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //services.Configure<EmailSettings>(emSet => Configuration.GetSection("EmailSettings").Bind(emSet));


            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IExperienceRepository, ExperienceRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            

            services.AddPaging();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            

            app.UseStaticFiles();
            //DbInitializer.Seed(serviceProvider.GetRequiredService<AppDbContext>());

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "experiencedetails",
                   template: "Experience/Details/{experienceID?}",
                   defaults: new { Controller = "Experience", action = "Details" });

                routes.MapRoute(
                    name: "paging",
                    template: "Experience/List/{page?}",
                    defaults: new { Controller = "Experience", action = "List" }
                    );

                routes.MapRoute(
                    name: "categoryfilter",
                    template: "Experience/{action}/{category?}",
                    defaults: new { Controller = "Experience", action = "List" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

        }
    }
}
