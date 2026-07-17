FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Administration.Api/LuxorLMS.Administration.Api.csproj", "src/LuxorLMS.Administration.Api/"]
COPY ["src/LuxorLMS.Administration.Application/LuxorLMS.Administration.Application.csproj", "src/LuxorLMS.Administration.Application/"]
COPY ["src/LuxorLMS.Administration.Domain/LuxorLMS.Administration.Domain.csproj", "src/LuxorLMS.Administration.Domain/"]
COPY ["src/LuxorLMS.Administration.Infrastructure/LuxorLMS.Administration.Infrastructure.csproj", "src/LuxorLMS.Administration.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Administration.Api/LuxorLMS.Administration.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Administration.Api"
RUN dotnet build "LuxorLMS.Administration.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Administration.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Administration.Api.dll"]
