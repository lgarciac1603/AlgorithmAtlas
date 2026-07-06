using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AlgorithmAtlas;
using AlgorithmAtlas.Services;
using AlgorithmAtlas.Services.NeuralNetworks;
using AlgorithmAtlas.Services.Sorting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<AlgorithmService>();
builder.Services.AddScoped<SoundService>();
builder.Services.AddScoped<ExoplanetDataService>();

await builder.Build().RunAsync();
