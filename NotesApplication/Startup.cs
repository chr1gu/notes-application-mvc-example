using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotesApplication.Extensions;
using NotesApplication.Services;

namespace NotesApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<INoteService, NoteService>();
            services.AddTransient<ITestDataService, TestDataService>();
            services.AddTransient<IUserSettingsService, UserSettingsService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<NoteDbContext>(opt => opt.UseInMemoryDatabase());

            services.AddSession();
            
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.ApplicationServices.GetService<ITestDataService>().RestoreTestData();
            
            app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");

            app.UseStaticFiles();
            
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Notes}/{action=Index}/{id?}");
            });
        }
    }
}