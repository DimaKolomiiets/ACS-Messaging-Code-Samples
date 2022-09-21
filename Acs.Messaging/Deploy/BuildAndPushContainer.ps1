# Subscription		    ACS Spool Testing 1
# Subscription ID		653983b8-683a-427c-8c27-9e9624ce9176
# Location			    East US
# ResourceGroup		    OzgursTestResourceGroup
# CommunicationService	OzgursTestACS
# ContainerRegistry 	ozgurscontainers
# AppService		    ozgurtest

$ContainerRegistry = "ozgurscontainers"
$DockerImage = "chat"
$ProjectFolder = "Acs.Messaging.Sample\Server"
$ProjectName = "Acs.Messaging.Sample.Server"

$version=$args[0]
if ($version.length -le 0) {
    Write-Warning "Version is not set using the version 'latest'`n"
    $version='latest'
}
Write-Host "`n`nVersion is set to >$version<"

Function Execute{
Param ($Value)
    Write-Host $Value -ForegroundColor DarkGreen
    iex $Value
}

Execute "az acr login --name $ContainerRegistry"

Write-Host "`n`nPublishing the server in release mode in the directory .\$ProjectFolder\bin\Release\net6.0"
Execute "dotnet publish $ProjectFolder\$ProjectName.csproj -c Release -o .\$ProjectFolder\bin\Release\net6.0"

Write-Host "`n`nBuilding the docker container"
Execute "docker build -f .\Deploy\Dockerfile -t ${ContainerRegistry}.azurecr.io/${DockerImage}:$version ."

Write-Host "`n`nPushing container to azure container registry"
Execute "docker push $ContainerRegistry.azurecr.io/${DockerImage}:$version"
