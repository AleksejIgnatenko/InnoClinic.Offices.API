# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["InnoClinic.Offices.API/InnoClinic.Offices.API.csproj", "InnoClinic.Offices.API/"]
COPY ["InnoClinic.Offices.Application/InnoClinic.Offices.Application.csproj", "InnoClinic.Offices.Application/"]
COPY ["InnoClinic.Offices.Core/InnoClinic.Offices.Core.csproj", "InnoClinic.Offices.Core/"]
COPY ["InnoClinic.Offices.DataAccess/InnoClinic.Offices.DataAccess.csproj", "InnoClinic.Offices.DataAccess/"]
COPY ["InnoClinic.Offices.Infrastructure/InnoClinic.Offices.Infrastructure.csproj", "InnoClinic.Offices.Infrastructure/"]

RUN dotnet restore "InnoClinic.Offices.API/InnoClinic.Offices.API.csproj"

COPY . .

WORKDIR "/src/InnoClinic.Offices.API"
RUN dotnet build "InnoClinic.Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "InnoClinic.Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InnoClinic.Offices.API.dll"]