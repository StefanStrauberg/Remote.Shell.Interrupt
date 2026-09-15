# syntax=docker/dockerfile:1

# --- Build ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/ src/

RUN dotnet restore "src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API/Remote.Shell.Interrupt.Storehouse.API.csproj"

RUN dotnet publish "src/Remote.Shell.Interrupt.Storehouse/Remote.Shell.Interrupt.Storehouse.API/Remote.Shell.Interrupt.Storehouse.API.csproj" \
    -c Release -o /app/publish --no-restore

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Used by the container HEALTHCHECK below to call the API's own liveness probe (/health/live).
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=3s --start-period=20s --retries=5 \
    CMD curl -f http://localhost:8080/health/live || exit 1

ENTRYPOINT ["dotnet", "Remote.Shell.Interrupt.Storehouse.API.dll"]
