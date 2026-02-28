FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Speculo.API/Speculo.API.csproj", "Speculo.API/"]
COPY ["Speculo.Application/Speculo.Application.csproj", "Speculo.Application/"]
COPY ["Speculo.Domain/Speculo.Domain.csproj", "Speculo.Domain/"]
COPY ["Speculo.Infrastructure/Speculo.Infrastructure.csproj", "Speculo.Infrastructure/"]
COPY ["Speculo.Contracts/Speculo.Contracts.csproj", "Speculo.Contracts/"]

RUN dotnet restore "Speculo.API/Speculo.API.csproj"

COPY . .
WORKDIR "/src/Speculo.API"
RUN dotnet publish "Speculo.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN adduser --disabled-password --gecos "" appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Speculo.API.dll"]
