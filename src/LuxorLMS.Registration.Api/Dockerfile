FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Registration.Api/LuxorLMS.Registration.Api.csproj", "src/LuxorLMS.Registration.Api/"]
COPY ["src/LuxorLMS.Registration.Application/LuxorLMS.Registration.Application.csproj", "src/LuxorLMS.Registration.Application/"]
COPY ["src/LuxorLMS.Registration.Domain/LuxorLMS.Registration.Domain.csproj", "src/LuxorLMS.Registration.Domain/"]
COPY ["src/LuxorLMS.Registration.Infrastructure/LuxorLMS.Registration.Infrastructure.csproj", "src/LuxorLMS.Registration.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Registration.Api/LuxorLMS.Registration.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Registration.Api"
RUN dotnet build "LuxorLMS.Registration.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Registration.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Registration.Api.dll"]
