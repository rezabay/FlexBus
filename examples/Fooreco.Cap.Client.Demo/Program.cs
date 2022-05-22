using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddRazorPages();

services.AddCap(x =>
{
    x.Version = configuration["Cap:DefaultVersion"];

    x.UsePostgreSql(configuration.GetConnectionString("Sql"));
});

var app = builder.Build();
app.UseDeveloperExceptionPage();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});

app.Run();