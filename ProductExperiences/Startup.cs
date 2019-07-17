using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductExperiences.Data;
using ProductExperiences.Data.Interfaces;
using ProductExperiences.Data.Mocks;
using ProductExperiences.Data.Repositories;

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

            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IExperienceRepository, ExperienceRepository>();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "experiencedetails",
                   template: "Experience/Details/{experienceID?}",
                   defaults: new { Controller = "Experience", action = "Details" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}");
            });

        }
    }
}
