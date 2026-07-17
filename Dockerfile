# Stage 1: Build React frontend
FROM node:20-alpine AS frontend-builder
WORKDIR /app/frontend
COPY frontend/package*.json ./
RUN npm ci --production=false
COPY frontend/ ./
RUN npm run build

# Stage 2: Build .NET API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-builder
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

# Stage 3: Final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copy .NET API
COPY --from=backend-builder /app/publish .

# Copy built frontend to wwwroot
COPY --from=frontend-builder /app/frontend/dist ./wwwroot/

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "LuxorLMS.Identity.Api.dll"]
