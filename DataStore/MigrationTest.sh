find . -type f -name *.NetCore.csproj -exec dotnet run --configuration Release --project {} --verbose \;
