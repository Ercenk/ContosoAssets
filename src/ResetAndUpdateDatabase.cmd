cd .\ContosoAssets.WebApp\

IF EXIST .\migrations (
    cd migrations
    del /s /q *.*
    cd ..
)

dotnet ef migrations add InitializeAssetManagement -c AssetManagementDbContext
dotnet ef database update -c AssetManagementDbContext

cd ..\ContosoAssets.SolutionManagement

IF EXIST .\migrations (
    cd migrations
    del /s /q *.*
    cd ..
)

dotnet ef --startup-project ..\ContosoAssets.WebApp migrations add Initialize -c AppIdentityDbContext
dotnet ef --startup-project ..\ContosoAssets.WebApp database update -c AppIdentityDbContext

dotnet ef --startup-project ..\ContosoAssets.WebApp  migrations add InitializeCustomerManagement -c CustomerManagementDbContext
dotnet ef --startup-project ..\ContosoAssets.WebApp  database update -c CustomerManagementDbContext

cd ..

