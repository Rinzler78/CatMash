import uuid;

Command="$1"
Name="$2"
Date=$(date +"%HH%MM%SS_%m-%d-%y")
DataStoreEFProjectPath="EF/CatMash.DataStore.EF"
MigrationIdFileName="CatmashDBContext+MigrationId.cs"
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
    echo "** EFInit : Name ($1), Source Project ($2)"
    
    name="$1"
    DBContextSourceProject="$2"
    MigrationPath=$DBContextSourceProject/Migrations
    
    echo "** EFInit : Delete migration path => $MigrationPath"
    
    rm -rf $MigrationPath
    
    EFAdd "$name" $DBContextSourceProject
}

function EFAdd(){
    echo "** EFAdd : Name ($1), Source Project ($2)"
    
    name="$1"
    DBContextSourceProject="$2"
    
    sourceProjectBasename=$(basename $DBContextSourceProject)
    echo "** EFAdd : Base name => $sourceProjectBasename"
    
    sourceProjectBaseDirectory=$(dirname $DBContextSourceProject)
    echo "** EFAdd : Base directory => $sourceProjectBaseDirectory"
    
    projectName=$sourceProjectBasename".csproj"
    echo "** EFAdd : Project name => $projectName"
    
    projetPath=$sourceProjectBaseDirectory/$sourceProjectBasename/$projectName
    echo "** EFAdd : Project path => $projetPath"

    StartupProjectName="Migrations.Helper.NetCore"
#    if [[ $DBContextSourceProject == *.Net ]]
#    then
#        StartupProjectName=$NetFramworkHelperProjectName
#    else
#        StartupProjectName="Migrations.Helper.NetCore"
#    fi
    
    StartupProjectDir=$sourceProjectBaseDirectory/$StartupProjectName
    StartupProjectDirPath=$StartupProjectDir/$StartupProjectName.csproj
    
    echo "** EFAdd : Startup project $StartupProjectDirPath"
    
    if [ ! -d $StartupProjectDir ]; then
        echo "** EFAdd : Create $StartupProjectDirPath"
        
        dotnet new console --name "$StartupProjectName" --output $StartupProjectDir
        
        dotnet add $StartupProjectDirPath package Microsoft.EntityFrameworkCore.Tools
        dotnet add $StartupProjectDirPath package Microsoft.EntityFrameworkCore.Design
        dotnet tool install --global dotnet-ef --version 3.0.0
        
        if [[ $StartupProjectName == *"SQLServer"* ]]; then
            dotnet add $StartupProjectDirPath package Microsoft.EntityFrameworkCore.SqlServer
        elif [[ $StartupProjectName == *"SQLite"* ]]; then
            dotnet add $StartupProjectDirPath package Microsoft.EntityFrameworkCore.Sqlite
        fi
    fi

#    pushd $projectDir
#
#    if [ ! -f ../$projetPath ]; then
#        echo "** EFAdd : ../$projetPath not exist"
#        exit
#    else
#        echo "** EFAdd : Add reference ../$projetPath"
#        dotnet add reference ../$projetPath
#    fi
#    popd

    dotnet add $StartupProjectDirPath reference $projetPath
    
    echo "** EFAdd : Restore packages => dotnet restore $StartupProjectName/$StartupProjectName.csproj"
    dotnet restore $StartupProjectDirPath
    
    echo "** EFAdd : Start Migration => dotnet ef migrations add "$name" --project $DBContextSourceProject --startup-project $StartupProjectName/$StartupProjectName.csproj --verbose"
    
    dotnet ef migrations add "$name" --project $DBContextSourceProject --startup-project $StartupProjectDirPath --verbose
}

if [ -z "$Name" ]; then
    Name="AutoMigrate_$(whoami)_$Date"
fi

echo "******************************************************"
echo "** Ef migration Helper $@"

for TargetEfProject in ${TargetEfProjects[*]} 
do
    echo "** Working on $TargetEfProject"

    MigrationPath="$TargetEfProject/Migrations"

    if [ -z "$Command" ]; then
        if [ ! -d "$MigrationPath" ]; then
            Command="Init"
        else
            Command="Add"
        fi
    fi

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

echo "******************************************************"
bash MigrationTest.sh
