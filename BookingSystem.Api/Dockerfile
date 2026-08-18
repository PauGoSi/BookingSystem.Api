FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BookingSystem.Api/BookingSystem.Api.csproj", "BookingSystem.Api/"]
RUN dotnet restore "BookingSystem.Api/BookingSystem.Api.csproj"

COPY . .
WORKDIR "/src/BookingSystem.Api"
RUN dotnet build "BookingSystem.Api.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "BookingSystem.Api.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BookingSystem.Api.dll"]