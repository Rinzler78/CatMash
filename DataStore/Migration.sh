import uuid;

Command="$1"
Name="$2"
Date=$(date +"%HH%MM%SS_%m-%d-%y")
DataStoreEFProjectPath="EF/CatMash.DataStore.EF"
MigrationIdFileName="GoodAngelDBContext+MigrationId.cs"
MigrationIdFilePath=$DataStoreEFProjectPath/$MigrationIdFileName
MigrationIdLinePrefix="public static readonly string MigrationId"

TargetEfProjects=(
    "EF/SQLite/CatMash.DataStore.EF.SQLite"
    )

function UpdateMigrationId()
{
    echo "Update Migration Id"
    echo "Target file $MigrationIdFilePath"
    CurrentMigrationIdLien=$(cat $MigrationIdFilePath | grep "$MigrationIdLinePrefix")
    echo "Current Migration id $CurrentMigrationIdLien"
    uuid=$(uuidgen)
    NewMigrationIdLinePrefix="$MigrationIdLinePrefix = \"$uuid\";"
    echo "New migration line $NewMigrationIdLinePrefix"
    sed -i "" "s/${CurrentMigrationIdLien}/${NewMigrationIdLinePrefix}/g" $MigrationIdFilePath

    echo "New migration line setted"
}

function EFInit ()
{
    name="$1"
    DBContextSourceProject="$2"
    MigrationPath=$DBContextSourceProject/Migrations

    echo "$DBContextSourceProject claim migration Init $name"
    echo "MigrationPath : $MigrationPath"

    rm -rf $MigrationPath
    
    EFAdd "$name" $DBContextSourceProject
}

function EFAdd(){
    name="$1"
    DBContextSourceProject="$2"
    sourceProjectBasename=$(basename $DBContextSourceProject)
    projectName=$(basename $DBContextSourceProject)".csproj"
    projetPath=$DBContextSourceProject/$projectName
    MigrationPath=$DBContextSourceProject/Migrations

    echo "$DBContextSourceProject claim migration Add $name"

    StartupProjectName=""
    if [[ $DBContextSourceProject == *.Net ]]
    then
        StartupProjectName=$NetFramworkHelperProjectName
    else
        StartupProjectName=$sourceProjectBasename".Migrations.Helper.NetCore"
    fi

    echo "Startup project $StartupProjectName"
    
    if [ ! -d $StartupProjectName ]; then
        echo "Create $StartupProjectName project"
        dotnet new console --name "$StartupProjectName"
        dotnet add $StartupProjectName/$StartupProjectName.csproj package Microsoft.EntityFrameworkCore.Tools

        if [[ $StartupProjectName == *"SQLServer"* ]]; then
            dotnet add $StartupProjectName/$StartupProjectName.csproj package Microsoft.EntityFrameworkCore.SqlServer
        elif [[ $StartupProjectName == *"SQLite"* ]]; then
            dotnet add $StartupProjectName/$StartupProjectName.csproj package Microsoft.EntityFrameworkCore.Sqlite
        fi
    fi

    pushd $StartupProjectName

    if [ ! -f ../$projetPath ]; then
        echo "../$projetPath not exist"
        exit
    else
        dotnet add reference ../$projetPath
    fi
    popd

    dotnet ef migrations add "$name" --project $DBContextSourceProject --startup-project $StartupProjectName/$StartupProjectName.csproj --verbose
}

if [ -z "$Name" ]; then
    Name="AutoMigrate_$(whoami)_$Date"
fi

echo "Ef migration Helper $Command $Name"
echo "DB Context project $DBContextSourceProject"

echo "*****************"

for TargetEfProject in ${TargetEfProjects[*]} 
do
    echo "Working on $TargetEfProject"

    MigrationPath="$TargetEfProject/Migrations"

    echo "Migration Path $MigrationPath"

    if [ -z "$Command" ]; then
        if [ ! -d "$MigrationPath" ]; then
            Command="Init"
        else
            Command="Add"
        fi
    fi

    echo "Using $Command"

    case "$Command" in
        "Init")
            EFInit "$Name (Init)" $TargetEfProject
        ;;
        "Add")
            EFAdd "$Name (Add)" $TargetEfProject
        ;;
    esac
    echo "*****************"
done

UpdateMigrationId

bash MigrationTest.sh
