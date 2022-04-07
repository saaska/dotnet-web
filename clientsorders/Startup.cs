using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;

using ClientsOrders.Models;

namespace ClientsOrders
{
    public class Startup
    {
        const string DBSERVER = "PGSQL";

        static Dictionary<string, Dictionary<string, string>> dbopt =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["PGSQL"] = new Dictionary<string, string>
                {
                    ["defdate"] = "NOW()",
                    ["locale"] = "ENCODING 'UTF8'"
                },
                ["MSSQL"] = new Dictionary<string, string>
                {
                    ["defdate"] = "CONVERT(datetime2(0),GETDATE())",
                    ["locale"] = "COLLATE Yakut_100_CI_AS_SC_UTF8"
                }
            };

        const string DATABASE_NAME = "backend";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var dbconn = Environment.GetEnvironmentVariable("DBCONN");
            if (dbconn == null)
            {
                Console.WriteLine("ERROR: Set database connection string in the DBCONN environment variable. Do not use database name, BACKEND will be used.");
                Environment.Exit(1);
            }
            dbconn += (dbconn.TrimEnd().EndsWith(';') ? "" : ";") + $"Database={DATABASE_NAME}";

            if (DBSERVER == "MSSQL")
                services.AddDbContext<MyDbContext>(
                    opt => opt.UseSqlServer(dbconn)
                );
            else
                services.AddDbContext<MyDbContext>(
                    opt => opt.UseNpgsql(dbconn)
                );

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1",
                    new Info {
                        Title = "Clients and Orders API",
                        Version = "v1",
                        Description = "Простой экспорт ASP.NET Core Web API в Swagger"
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clients and Orders API V1");
                });
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
