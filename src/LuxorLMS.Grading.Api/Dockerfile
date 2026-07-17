FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Grading.Api/LuxorLMS.Grading.Api.csproj", "src/LuxorLMS.Grading.Api/"]
COPY ["src/LuxorLMS.Grading.Application/LuxorLMS.Grading.Application.csproj", "src/LuxorLMS.Grading.Application/"]
COPY ["src/LuxorLMS.Grading.Domain/LuxorLMS.Grading.Domain.csproj", "src/LuxorLMS.Grading.Domain/"]
COPY ["src/LuxorLMS.Grading.Infrastructure/LuxorLMS.Grading.Infrastructure.csproj", "src/LuxorLMS.Grading.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Grading.Api/LuxorLMS.Grading.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Grading.Api"
RUN dotnet build "LuxorLMS.Grading.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Grading.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Grading.Api.dll"]
