# Stage 1: restore + publish
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so restore is cached independently of source changes
COPY CRM.API/CRM.API.csproj      CRM.API/
COPY CRM.Base/CRM.Base.csproj    CRM.Base/
COPY CRM.Data/CRM.Data.csproj    CRM.Data/
COPY CRM.Domain/CRM.Domain.csproj CRM.Domain/
COPY CRM.Service/CRM.Services.csproj CRM.Service/
RUN dotnet restore CRM.API/CRM.API.csproj

COPY . .
RUN dotnet publish CRM.API/CRM.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/publish .

# SQLite database lives on a volume so it survives container restarts
RUN mkdir -p /app/data
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CRM.API.dll"]
