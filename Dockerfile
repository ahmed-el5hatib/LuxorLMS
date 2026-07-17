FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Identity.Api/LuxorLMS.Identity.Api.csproj", "src/LuxorLMS.Identity.Api/"]
COPY ["src/LuxorLMS.Identity.Application/LuxorLMS.Identity.Application.csproj", "src/LuxorLMS.Identity.Application/"]
COPY ["src/LuxorLMS.Identity.Domain/LuxorLMS.Identity.Domain.csproj", "src/LuxorLMS.Identity.Domain/"]
COPY ["src/LuxorLMS.Identity.Infrastructure/LuxorLMS.Identity.Infrastructure.csproj", "src/LuxorLMS.Identity.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Identity.Api/LuxorLMS.Identity.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Identity.Api"
RUN dotnet publish "LuxorLMS.Identity.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS frontend
COPY src/LuxorLMS.Frontend/ /usr/share/nginx/html/

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=frontend /usr/share/nginx/html/ ./wwwroot/
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LuxorLMS.Identity.Api.dll"]
