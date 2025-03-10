FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SocketheadCleanArch.Admin/SocketheadCleanArch.Admin.csproj", "SocketheadCleanArch.Admin/"]
RUN dotnet restore "SocketheadCleanArch.Admin/SocketheadCleanArch.Admin.csproj"
COPY . .
WORKDIR "/src/SocketheadCleanArch.Admin"
RUN dotnet build "SocketheadCleanArch.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SocketheadCleanArch.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocketheadCleanArch.Admin.dll"]
