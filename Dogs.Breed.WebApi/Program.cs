using Dogs.Breed.WebApi.ML;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Dogs.Breed.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Task.Factory.StartNew(() => new DogsTrainingEngine().BuildAndSaveModel());
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
