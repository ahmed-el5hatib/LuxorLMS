FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Notifications.Api/LuxorLMS.Notifications.Api.csproj", "src/LuxorLMS.Notifications.Api/"]
COPY ["src/LuxorLMS.Notifications.Application/LuxorLMS.Notifications.Application.csproj", "src/LuxorLMS.Notifications.Application/"]
COPY ["src/LuxorLMS.Notifications.Domain/LuxorLMS.Notifications.Domain.csproj", "src/LuxorLMS.Notifications.Domain/"]
COPY ["src/LuxorLMS.Notifications.Infrastructure/LuxorLMS.Notifications.Infrastructure.csproj", "src/LuxorLMS.Notifications.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Notifications.Api/LuxorLMS.Notifications.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Notifications.Api"
RUN dotnet build "LuxorLMS.Notifications.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Notifications.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Notifications.Api.dll"]
