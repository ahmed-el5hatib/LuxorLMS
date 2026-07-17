FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Reporting.Api/LuxorLMS.Reporting.Api.csproj", "src/LuxorLMS.Reporting.Api/"]
COPY ["src/LuxorLMS.Reporting.Application/LuxorLMS.Reporting.Application.csproj", "src/LuxorLMS.Reporting.Application/"]
COPY ["src/LuxorLMS.Reporting.Domain/LuxorLMS.Reporting.Domain.csproj", "src/LuxorLMS.Reporting.Domain/"]
COPY ["src/LuxorLMS.Reporting.Infrastructure/LuxorLMS.Reporting.Infrastructure.csproj", "src/LuxorLMS.Reporting.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Reporting.Api/LuxorLMS.Reporting.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Reporting.Api"
RUN dotnet build "LuxorLMS.Reporting.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Reporting.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Reporting.Api.dll"]
