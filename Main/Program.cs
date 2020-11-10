using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Kraken.Api.Main
{
    /// <summary>
    /// Class containing the main entry point of the program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry point of the program.
        /// </summary>
        /// <param name="args">(Optional) command line parameters.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates a new host builder and ensures that our Startup class is used.
        /// </summary>
        /// <param name="args">(Optional) command line parameters.</param>
        /// <returns>A host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
