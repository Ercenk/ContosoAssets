FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2.300-stretch AS build
WORKDIR /src
COPY ["src/ContosoAssets.AMPIntegration/ContosoAssets.AMPIntegration.csproj", "src/ContosoAssets.AMPIntegration/"]
COPY ["src/ContosoAssets.Utils/ContosoAssets.Utils.csproj", "src/ContosoAssets.Utils/"]
COPY ["src/ContosoAssets.SolutionManagement/ContosoAssets.SolutionManagement.csproj", "src/ContosoAssets.SolutionManagement/"]
RUN dotnet restore "src/ContosoAssets.AMPIntegration/ContosoAssets.AMPIntegration.csproj"
COPY . .
WORKDIR "/src/src/ContosoAssets.AMPIntegration"
RUN dotnet build "ContosoAssets.AMPIntegration.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContosoAssets.AMPIntegration.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ContosoAssets.AMPIntegration.dll"]
