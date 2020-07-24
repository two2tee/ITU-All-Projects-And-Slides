// Program.cs is part of LiftLog and was created on 04/08/2017. 
// Last modified on 04/15/2017.

using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace LiftLog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}