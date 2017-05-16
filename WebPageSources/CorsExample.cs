
using System.Collections.Generic;
using System.IO;
using CoreRepo.Data;
using CoreRepo.DataAccess;
using CoreRepo.DataAccess.Account;
using CoreRepo.DataAccess.Orders;
using CoreRepo.DataAccess.Work;
using CoreRepo.DataAccess.Work.Expenses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services;

namespace B2kConstructionApi
{//begin namespace

    public class Startup
    {//begin class

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {//begin constructor
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }//end constructor

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {//begin method

            //services.AddDbContext<Context>(options => options.UseSqlServer(Configuration.GetConnectionString("LiveConnection")));
            services.AddDbContext<Context>(options => options.UseSqlServer(Configuration.GetConnectionString("LocalHostConnection")));
   
            services.AddScoped<DataAccess>();
            // Account
            services.AddScoped<UserDataAccess>();
            services.AddScoped<SessionDataAccess>();
            services.AddScoped<UsersRolesDataAccess>();
            services.AddScoped<RoleDataAccess>();
            // Order
            services.AddScoped<OrderRequestDataAccess>();
            services.AddScoped<OrderDataAccess>();
            //Work
            services.AddScoped<WorkSessionDataAccess>();
            services.AddScoped<WorkImageDataAccess>();
            services.AddScoped<ExpensesDataAccess>();
            services.AddScoped<ExpenseTypeDataAccess>();
            services.AddScoped<ImageTypeDataAccess>();

            // Add framework services.
            services.AddMvc();

            services.AddCors(o => o.AddPolicy("allowAll", builder =>
            {//begin define
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));//end define
            services.AddCors(o => o.AddPolicy("allowLiveSite", builder =>
            {//begin define
                builder.WithOrigins("http://b2kconstruction.com")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));//end define
            services.AddCors(o => o.AddPolicy("localHost", builder =>
            {//begin define
                builder.WithOrigins("http://localhost:4200")
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));//end define
        }//end method

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Context context)
        {//beign method
            app.UseCors("allowAll");
            app.UseCors("localHost");

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.Use(async (routecontext, next) =>
            {//begin define
                await next();
                if (routecontext.Response.StatusCode == 404 && !Path.HasExtension(routecontext.Request.Path.Value))
                {//begin if
                    routecontext.Request.Path = "/index.html";
                    await next();
                }//end if
            });//end define
            app.UseDefaultFiles(new DefaultFilesOptions { DefaultFileNames = new List<string> { "index.html" } })
                .UseStaticFiles()
                .UseMvc();

            DbInitializer.Init(context);
        }//end method

    }//end class

}//end namespace