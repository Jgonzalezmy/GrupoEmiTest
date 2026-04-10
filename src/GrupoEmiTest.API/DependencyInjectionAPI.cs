using FluentValidation;
using GrupoEmiTest.API.Policies;
using GrupoEmiTest.Application.Settings;
using GrupoEmiTest.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using System.Text;
using System.Text.Json.Serialization;

namespace GrupoEmiTest.API;
public static class DependencyInjectionAPI
{
    public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()!;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                };
            });

        // Authorization policies 
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyConstants.ReadPolicy, policy =>
                policy.RequireRole(nameof(RoleType.Admin), nameof(RoleType.User)));

            options.AddPolicy(PolicyConstants.WritePolicy, policy =>
                policy.RequireRole(nameof(RoleType.Admin)));

            options.AddPolicy(PolicyConstants.EditPolicy, policy =>
                policy.RequireRole(nameof(RoleType.Admin)));

            options.AddPolicy(PolicyConstants.DeletePolicy, policy =>
                policy.RequireRole(nameof(RoleType.Admin)));
        });

        //FluentValidator
        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddFluentValidationAutoValidation();

        services.AddOpenApi();

        return services;
    }
}