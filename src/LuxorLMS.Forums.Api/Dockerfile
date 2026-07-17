FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Forums.Api/LuxorLMS.Forums.Api.csproj", "src/LuxorLMS.Forums.Api/"]
COPY ["src/LuxorLMS.Forums.Application/LuxorLMS.Forums.Application.csproj", "src/LuxorLMS.Forums.Application/"]
COPY ["src/LuxorLMS.Forums.Domain/LuxorLMS.Forums.Domain.csproj", "src/LuxorLMS.Forums.Domain/"]
COPY ["src/LuxorLMS.Forums.Infrastructure/LuxorLMS.Forums.Infrastructure.csproj", "src/LuxorLMS.Forums.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Forums.Api/LuxorLMS.Forums.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Forums.Api"
RUN dotnet build "LuxorLMS.Forums.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Forums.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Forums.Api.dll"]
