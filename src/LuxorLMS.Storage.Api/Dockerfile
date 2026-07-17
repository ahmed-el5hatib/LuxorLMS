FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Storage.Api/LuxorLMS.Storage.Api.csproj", "src/LuxorLMS.Storage.Api/"]
COPY ["src/LuxorLMS.Storage.Application/LuxorLMS.Storage.Application.csproj", "src/LuxorLMS.Storage.Application/"]
COPY ["src/LuxorLMS.Storage.Domain/LuxorLMS.Storage.Domain.csproj", "src/LuxorLMS.Storage.Domain/"]
COPY ["src/LuxorLMS.Storage.Infrastructure/LuxorLMS.Storage.Infrastructure.csproj", "src/LuxorLMS.Storage.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Storage.Api/LuxorLMS.Storage.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Storage.Api"
RUN dotnet build "LuxorLMS.Storage.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Storage.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Storage.Api.dll"]
