FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Money.API/Money.API.csproj", "Money.API/"]
COPY ["Money.BL/Money.BL.csproj", "Money.BL/"]
COPY ["Money.Common/Money.Common.csproj", "Money.Common/"]
COPY ["Money.Infrastructure/Money.Infrastructure.csproj", "Money.Infrastructure/"]
RUN dotnet restore "./Money.API/Money.API.csproj"
COPY . .
WORKDIR "/src/Money.API"
RUN dotnet build "./Money.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Money.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Money.API.dll"]