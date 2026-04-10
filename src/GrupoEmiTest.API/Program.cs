using GrupoEmiTest.API.Middlewares.Extensions;
using GrupoEmiTest.Application;
using GrupoEmiTest.Infrastructure;
using GrupoEmiTest.Infrastructure.Extensions;
using Scalar.AspNetCore;

namespace GrupoEmiTest.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAPIServices(builder.Configuration)
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        if (!app.Environment.IsDevelopment())
            app.UseHttpsRedirection();

        await app.InitialiseDatabaseAsync();
        app.UseRequestLogging();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
