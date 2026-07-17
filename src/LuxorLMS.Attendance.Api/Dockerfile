FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Attendance.Api/LuxorLMS.Attendance.Api.csproj", "src/LuxorLMS.Attendance.Api/"]
COPY ["src/LuxorLMS.Attendance.Application/LuxorLMS.Attendance.Application.csproj", "src/LuxorLMS.Attendance.Application/"]
COPY ["src/LuxorLMS.Attendance.Domain/LuxorLMS.Attendance.Domain.csproj", "src/LuxorLMS.Attendance.Domain/"]
COPY ["src/LuxorLMS.Attendance.Infrastructure/LuxorLMS.Attendance.Infrastructure.csproj", "src/LuxorLMS.Attendance.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Attendance.Api/LuxorLMS.Attendance.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Attendance.Api"
RUN dotnet build "LuxorLMS.Attendance.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Attendance.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Attendance.Api.dll"]
