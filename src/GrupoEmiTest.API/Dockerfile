FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/GrupoEmiTest.API/GrupoEmiTest.API.csproj", "src/GrupoEmiTest.API/"]
COPY ["src/GrupoEmiTest.Application/GrupoEmiTest.Application.csproj", "src/GrupoEmiTest.Application/"]
COPY ["src/GrupoEmiTest.Domain/GrupoEmiTest.Domain.csproj", "src/GrupoEmiTest.Domain/"]
COPY ["src/GrupoEmiTest.Infrastructure/GrupoEmiTest.Infrastructure.csproj", "src/GrupoEmiTest.Infrastructure/"]
RUN dotnet restore "./src/GrupoEmiTest.API/GrupoEmiTest.API.csproj"
COPY . .
WORKDIR "/src/src/GrupoEmiTest.API"

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GrupoEmiTest.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GrupoEmiTest.API.dll"]
