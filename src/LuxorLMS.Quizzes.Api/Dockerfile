FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/LuxorLMS.Quizzes.Api/LuxorLMS.Quizzes.Api.csproj", "src/LuxorLMS.Quizzes.Api/"]
COPY ["src/LuxorLMS.Quizzes.Application/LuxorLMS.Quizzes.Application.csproj", "src/LuxorLMS.Quizzes.Application/"]
COPY ["src/LuxorLMS.Quizzes.Domain/LuxorLMS.Quizzes.Domain.csproj", "src/LuxorLMS.Quizzes.Domain/"]
COPY ["src/LuxorLMS.Quizzes.Infrastructure/LuxorLMS.Quizzes.Infrastructure.csproj", "src/LuxorLMS.Quizzes.Infrastructure/"]
COPY ["src/LuxorLMS.Kernel/LuxorLMS.Kernel.csproj", "src/LuxorLMS.Kernel/"]
RUN dotnet restore "src/LuxorLMS.Quizzes.Api/LuxorLMS.Quizzes.Api.csproj"
COPY . .
WORKDIR "/src/src/LuxorLMS.Quizzes.Api"
RUN dotnet build "LuxorLMS.Quizzes.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuxorLMS.Quizzes.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuxorLMS.Quizzes.Api.dll"]
