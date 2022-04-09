using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClientsOrders
{
    public class Program
    {
        static string port = Environment.GetEnvironmentVariable("PORT");
        static string host = String.IsNullOrEmpty(port) ? "" : "http://0.0.0.0:" + port;
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var b = WebHost.CreateDefaultBuilder(args);
            return host=="" ? b.UseStartup<Startup>() 
                            : b.UseStartup<Startup>().UseUrls(new string [] {host});
        }
        
    }
}
