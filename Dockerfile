FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Speculo.Tracking/Speculo.API/Speculo.API.csproj", "Speculo.Tracking/Speculo.API/"]
COPY ["Speculo.Tracking/Speculo.Application/Speculo.Application.csproj", "Speculo.Tracking/Speculo.Application/"]
COPY ["Speculo.Tracking/Speculo.Domain/Speculo.Domain.csproj", "Speculo.Tracking/Speculo.Domain/"]
COPY ["Speculo.Tracking/Speculo.Infrastructure/Speculo.Infrastructure.csproj", "Speculo.Tracking/Speculo.Infrastructure/"]
COPY ["Speculo.Contracts/Speculo.Contracts.csproj", "Speculo.Contracts/"]

RUN dotnet restore "Speculo.Tracking/Speculo.API/Speculo.API.csproj"

COPY . .
WORKDIR "/src/Speculo.Tracking/Speculo.API"
RUN dotnet publish "Speculo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Speculo.API.dll"]
