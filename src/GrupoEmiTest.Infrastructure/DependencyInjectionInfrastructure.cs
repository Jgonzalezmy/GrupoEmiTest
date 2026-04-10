using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Domain.Interfaces;
using GrupoEmiTest.Infrastructure.Persistence;
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
