
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

            app.UseMvc();

            DbInitializer.Init(context);
        }//end method

    }//end class

}//end namespace