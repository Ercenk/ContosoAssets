FROM mcr.microsoft.com/dotnet/core/sdk:2.2.300-stretch AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2.300-stretch AS build
WORKDIR /src
COPY ["src/ContosoAssets.WebApp/ContosoAssets.WebApp.csproj", "src/ContosoAssets.WebApp/"]
COPY ["src/ContosoAssets.WebApp/createDatabase.sh", "src/ContosoAssets.WebApp/"]
COPY ["src/ContosoAssets.Utils/ContosoAssets.Utils.csproj", "src/ContosoAssets.Utils/"]
COPY ["src/ContosoAssets.SolutionManagement/ContosoAssets.SolutionManagement.csproj", "src/ContosoAssets.SolutionManagement/"]
RUN dotnet restore "src/ContosoAssets.WebApp/ContosoAssets.WebApp.csproj"
COPY . .
WORKDIR "/src/src/ContosoAssets.WebApp"
RUN dotnet build "ContosoAssets.WebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ContosoAssets.WebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY --from=publish /src /src
RUN chmod +x /src/src/ContosoAssets.WebApp/createDatabase.sh
ENTRYPOINT ["/src/src/ContosoAssets.WebApp/createDatabase.sh"]