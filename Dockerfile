FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Speculo.API/Speculo.API.csproj", "Speculo.API/"]
COPY ["Speculo.Application/Speculo.Application.csproj", "Speculo.Application/"]
COPY ["Speculo.Domain/Speculo.Domain.csproj", "Speculo.Domain/"]
COPY ["Speculo.Infrastructure/Speculo.Infrastructure.csproj", "Speculo.Infrastructure/"]

RUN dotnet restore "Speculo.API/Speculo.API.csproj"

COPY . .
WORKDIR "/src/Speculo.API"
RUN dotnet build "Speculo.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Speculo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Speculo.API.dll"]
