# 1. Építési fázis (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Másoljuk a csproj fájlt és telepítjük a függőségeket
# FIGYELEM: Ha a .csproj fájlod neve nem 'foodshop.csproj', írd át a következő 2 sorban!
COPY ["foodshop.csproj", "./"]
RUN dotnet restore "./foodshop.csproj"

# Másoljuk a többi fájlt és lefordítjuk
COPY . .
RUN dotnet build "foodshop.csproj" -c Release -o /app/build

# 2. Közzététel (Publish)
FROM build AS publish
RUN dotnet publish "foodshop.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Futási fázis (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .
# FIGYELEM: Itt is írd át a 'foodshop.dll'-t a projekted nevére, ha más!
ENTRYPOINT ["dotnet", "foodshop.dll"]