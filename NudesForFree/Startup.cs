using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NudesForFree.Repositories;
using NudesForFree.Services;

namespace NudesForFree
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
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0).AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.IgnoreNullValues = true;
                opt.JsonSerializerOptions.PropertyNameCaseInsensitive = false;                
            });
            services.AddControllers();
            services.AddSingleton<UserService>();
            services.AddSingleton<PostService>();
            services.AddSingleton<InteractionService>();
            services.AddSingleton<CommentService>();
            services.AddSingleton<UserRepository>();
            services.AddSingleton<PostRepository>();
            services.AddSingleton<InteractionRepository>();
            services.AddSingleton<CommentRepository>();

			services.AddCors(options =>
			{
				options.AddPolicy("AllowAllHeaders",
					  builder =>
					  {
						  builder.AllowAnyOrigin()
								 .AllowAnyHeader()
								 .AllowAnyMethod();
					  });
			});
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
                app.UseExceptionHandler("/Home/Error");
            }
          
            app.UseRouting();

            app.UseAuthorization();
			app.UseStaticFiles();

			app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");                
            });
			app.UseCors("AllowAllHeaders");
		}
    }
}
