using GrupoEmiTest.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace GrupoEmiTest.Infrastructure.Extensions;

public static class DataBaseExtension
{
    /// <summary>
    /// Applies pending EF Core migrations and seeds initial data into the database.
    /// </summary>
    public static async Task InitialiseDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateAsyncScope();

        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }
}
