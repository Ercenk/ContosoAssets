#!/bin/bash

set -e
cd /src/src/ContosoAssets.WebApp

if [ -d "Migrations" ]; then
    cd Migrations
    rm -f -r *
    cd ..
fi

dotnet ef migrations add InitializeAssetManagement -c AssetManagementDbContext
dotnet ef database update -c AssetManagementDbContext

cd ../ContosoAssets.SolutionManagement

if [ -d "Migrations" ]; then
    cd Migrations
    rm -f -r *
    cd ..
fi

dotnet ef --startup-project ../ContosoAssets.WebApp migrations add Initialize -c AppIdentityDbContext
dotnet ef --startup-project ../ContosoAssets.WebApp database update -c AppIdentityDbContext

dotnet ef --startup-project ../ContosoAssets.WebApp  migrations add InitializeCustomerManagement -c CustomerManagementDbContext
dotnet ef --startup-project ../ContosoAssets.WebApp  database update -c CustomerManagementDbContext
cd /app
dotnet ContosoAssets.WebApp.dll