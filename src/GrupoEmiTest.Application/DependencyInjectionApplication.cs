using GrupoEmiTest.Application.Interfaces;
using GrupoEmiTest.Application.Services;
using GrupoEmiTest.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GrupoEmiTest.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();

        return services;
    }
}
