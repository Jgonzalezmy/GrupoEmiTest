
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

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        await app.InitialiseDatabaseAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())        
            app.MapOpenApi();

        app.MapScalarApiReference();
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
