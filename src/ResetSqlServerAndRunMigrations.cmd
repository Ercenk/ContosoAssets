docker container stop sqldatabase
docker container rm sqldatabase
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=V3ryS3cr3tP@ssw0rd" -p 1433:1433 --name sqldatabase -d mcr.microsoft.com/mssql/server:2017-latest
.\ResetAndUpdateDatabase.cmd