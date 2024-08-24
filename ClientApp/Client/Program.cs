using ClientApp.Services;
using InsuranceApi.DTOs;
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

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5122/api/") });
            builder.Services.AddScoped <IInsuranceTypeDtoService, InsuranceTypeDtoService> ();
            builder.Services.AddScoped<IPolicyHolderDtoService, PolicyHolderDtoService>();
            builder.Services.AddScoped<IInsuredDtoService, InsuredDtoService>();
            builder.Services.AddScoped<IPaymentDtoService, PaymentDtoService>();
            builder.Services.AddScoped<IPolicyDtoService, PolicyDtoService>();
            builder.Services.AddScoped<AuthService>();
            await builder.Build().RunAsync();
        }
    }
}
