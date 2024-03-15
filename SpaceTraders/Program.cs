using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SpaceTradersApp.Data;
using System.Net.Http.Headers;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<AgentService>();
builder.Services.AddSingleton<LocationService>();
builder.Services.AddScoped<HttpClient>();
builder.Services.AddSingleton<RateLimitedQueueManager>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient<SpaceTradersClient>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
