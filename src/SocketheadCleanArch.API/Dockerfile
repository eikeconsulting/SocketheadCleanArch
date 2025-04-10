FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
# https is not supported in the base image
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SocketheadCleanArch.API/SocketheadCleanArch.API.csproj", "SocketheadCleanArch.API/"]
RUN dotnet restore "SocketheadCleanArch.API/SocketheadCleanArch.API.csproj"
COPY . .
WORKDIR "/src/SocketheadCleanArch.API"
RUN dotnet build "SocketheadCleanArch.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SocketheadCleanArch.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy all appsettings files from the build stage
COPY --from=build /src/appsettings*.json ./
ENTRYPOINT ["dotnet", "SocketheadCleanArch.API.dll"]
