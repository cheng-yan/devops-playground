# Build stage: full SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Playground.Api/Playground.Api.csproj src/Playground.Api/
RUN dotnet restore src/Playground.Api/Playground.Api.csproj
COPY src/ src/
RUN dotnet publish src/Playground.Api/Playground.Api.csproj -c Release -o /app --no-restore /p:UseAppHost=false

# Runtime stage: chiseled image (no shell, no package manager, non-root user)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Playground.Api.dll"]
