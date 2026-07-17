FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Analytics.Api/LuxorLMS.Analytics.Api.csproj", "src/LuxorLMS.Analytics.Api/"]
COPY ["src/LuxorLMS.Analytics.Application/LuxorLMS.Analytics.Application.csproj", "src/LuxorLMS.Analytics.Application/"]
COPY ["src/LuxorLMS.Analytics.Domain/LuxorLMS.Analytics.Domain.csproj", "src/LuxorLMS.Analytics.Domain/"]
COPY ["src/LuxorLMS.Analytics.Infrastructure/LuxorLMS.Analytics.Infrastructure.csproj", "src/LuxorLMS.Analytics.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Analytics.Api/LuxorLMS.Analytics.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Analytics.Api"
RUN dotnet build "LuxorLMS.Analytics.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Analytics.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Analytics.Api.dll"]
