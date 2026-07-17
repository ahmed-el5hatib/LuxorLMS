FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Academic.Api/LuxorLMS.Academic.Api.csproj", "src/LuxorLMS.Academic.Api/"]
COPY ["src/LuxorLMS.Academic.Application/LuxorLMS.Academic.Application.csproj", "src/LuxorLMS.Academic.Application/"]
COPY ["src/LuxorLMS.Academic.Domain/LuxorLMS.Academic.Domain.csproj", "src/LuxorLMS.Academic.Domain/"]
COPY ["src/LuxorLMS.Academic.Infrastructure/LuxorLMS.Academic.Infrastructure.csproj", "src/LuxorLMS.Academic.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Academic.Api/LuxorLMS.Academic.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Academic.Api"
RUN dotnet build "LuxorLMS.Academic.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Academic.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Academic.Api.dll"]
