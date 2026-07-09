FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy NuGet config first
COPY nuget.config .

# Copy project files and restore
COPY *.sln .
COPY src/OrderFlow.Domain/*.csproj src/OrderFlow.Domain/
COPY src/OrderFlow.Application/*.csproj src/OrderFlow.Application/
COPY src/OrderFlow.Infrastructure/*.csproj src/OrderFlow.Infrastructure/
COPY src/OrderFlow.Api/*.csproj src/OrderFlow.Api/
RUN dotnet restore src/OrderFlow.Api/OrderFlow.Api.csproj

# Copy source and publish
COPY . .
RUN dotnet publish src/OrderFlow.Api/OrderFlow.Api.csproj \
    -c Release \
    -o /app/publish

# ── Runtime ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderFlow.Api.dll"]