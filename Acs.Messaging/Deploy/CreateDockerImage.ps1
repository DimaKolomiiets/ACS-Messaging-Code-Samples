$ContainerRegistry = "ozgurscontainers"
$DockerImage = "chat"
$ProjectFolder = "Acs.Messaging.Sample\Server"
$ProjectName = "Acs.Messaging.Sample.Server"
$version = "v0.1"

Function Execute{
Param ($Value)
    Write-Host $Value -ForegroundColor DarkGreen
    iex $Value
}

Execute "az acr login --name $ContainerRegistry"

Write-Host "`n`nPublishing the server in release mode in the directory .\$ProjectFolder\bin\Release\net6.0"
Execute "dotnet publish $ProjectFolder\$ProjectName.csproj -c Release -o .\$ProjectFolder\bin\Release\net6.0"

Write-Host "`n`nBuilding the docker container"
Execute "docker build -f .\deploy\Dockerfile -t ${DockerImage}:$version ."
