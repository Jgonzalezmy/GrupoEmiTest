using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Application.Settings;
using GrupoEmiTest.Domain.Interfaces;
using GrupoEmiTest.Infrastructure.Data;
using GrupoEmiTest.Infrastructure.Repositories;
using GrupoEmiTest.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrupoEmiTest.Infrastructure;

public static class DependencyInjectionInfrastructure
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

        var conectionString = configuration.GetConnectionString("SqlServer");
        services.AddDbContext<GrupoEmiTestDBContext>(options =>
            options.UseSqlServer(conectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IPositionHistoryRepository, PositionHistoryRepository>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<DataSeeder>();

        return services;
    }

}
