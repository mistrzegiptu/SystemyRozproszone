# See https://aka.ms/customizecontainer to learn how to customize your debug container 

# This stage is used when running from VS in fast mode (Default for Debug configuration)
# Using '10.0' or '10.0-bookworm-slim' targets Linux by default
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CineMatch.csproj", "."]
RUN dotnet restore "./CineMatch.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./CineMatch.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CineMatch.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for production
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CineMatch.dll"]