using ClientApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ClientApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5006/api/") });
            builder.Services.AddScoped <IInsuranceTypeDtoService, InsuranceTypeDtoService> ();
            builder.Services.AddScoped<IPolicyHolderDtoService, PolicyHolderDtoService>();

            await builder.Build().RunAsync();
        }
    }
}
