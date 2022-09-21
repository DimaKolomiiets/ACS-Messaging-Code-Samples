
# ~~Create Azure Communication Services~~

# Create Azure Respources

1. Open Azure Portal (https://portal.azure.com)

2. Create Container Registry
   1. Search for ***Container Registries***
   2. Create a Container Registry

      > Note `Registry Name` on this page as **`ContainerRegistry`**

   3. After Container Registry is created, enable admin user for Container Registry

3. Web App
   1. Search for ***App Services***
   2. Create a Web App
      1. Set *Basics | Publish* option as `Docker Container`

         > Note `Name` on this page as **`AppName`**

      2. Set *Docker | Image Source* option as `Azure container Registry`

         > Note `Image` on this page as **`DockerImage`**

      3. Set *Docker | Tag* option as your first version (e.g `v0.1`)

         > Note `Tag` on this page as **`AppVersion`**

# Docker

1. Ensure the `Docker Desktop` software is running on your computer
2. Install Azure CLI ([Azure CLI on Windows](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli))

## Create Docker File

1. Open command line (cmd)
2. Go to solution directory (*e.g C:\Projects\Hot\Acs.Messaging\*)
3. Create deploy folder & dockerfile
   ```cmd
   mkdir Deploy
   cd Deploy
   type nul > dockerfile
   notepad dockerfile
   ```
4. Type following and save
   ```docker
   FROM mcr.microsoft.com/dotnet/aspnet:6.0
   COPY ./Acs.Messaging.Sample/Server/bin/Release/net6.0 /app
   WORKDIR /app
   EXPOSE 80
   EXPOSE 443
   ENTRYPOINT ["dotnet", "Acs.Messaging.Sample.Server.dll"]
   ```

## Docker Image

1. Login to Azure
   ```powershell
   az login
   ```
2. Login to Azure container registry using **`ContainerRegistry`**
   ```powershell
   az acr login --name [ContainerRegistry]
   az acr login --name ozgurscontainers
   ```
3. Build your project
   ```cmd
   dotnet publish Acs.Messaging.Sample\Server\Acs.Messaging.Sample.Server.csproj -c Release -o .\Acs.Messaging.Sample\Server\bin\Release\net6.0
   ```
4. Build Docker image for Azure using **`DockerImage`** and **`AppVersion`**
   ```cmd
   docker build -f .\Deploy\Dockerfile -t [DockerImage]:[AppVersion] .
   docker build -f .\Deploy\Dockerfile -t chat:v0.1 .
   ```

## Deploy to Azure

1. Open PowerShell console
2. Go to solution directory (*e.g C:\Projects\Hot\Acs.Messaging\*)
3. Login to Azure
   ```powershell
   az login
   ```
4. Login to Azure container registry using **`ContainerRegistry`**
   ```powershell
   az acr login --name [ContainerRegistry]
   az acr login --name ozgurscontainers
   ```
5. Build your project
   ```cmd
   dotnet publish Acs.Messaging.Sample\Server\Acs.Messaging.Sample.Server.csproj -c Release -o .\Acs.Messaging.Sample\Server\bin\Release\net6.0
   ```
6. Build Docker image for Azure using **`ContainerRegistry`**, **`DockerImage`** and **`AppVersion`**
   ```cmd
   docker build -f .\Deploy\Dockerfile -t [ContainerRegistry].azurecr.io/[DockerImage]:[AppVersion] .
   docker build -f .\Deploy\Dockerfile -t ozgurscontainers.azurecr.io/chat:v0.1 .
   ```
7. Push to Azure Container Registry using **`ContainerRegistry`**, **`DockerImage`** and **`AppVersion`**
   ```cmd
   docker push [ContainerRegistry].azurecr.io/[DockerImage]:[AppVersion]
   docker push ozgurscontainers.azurecr.io/chat:v0.1
   ```
8. Wait for about ~5 minutes for Azure to deploy your changes.

9. The app is available at [https://**`[AppName]`**.azurewebsites.net](https://[AppName].azurewebsites.net)

   https://ozgurtest.azurewebsites.net

# Manual Azure Deploy (If you don't want to wait about ~5 minutes)

1. Open Azure Portal (https://portal.azure.com)
2. Go to your **App Service**
5. Under **Deployment** click **Deployment Center**
6. Under **Settings** change **Tag** as your latest version number
7. Click **Save**

Event Web Hook Subscriber Endpoint
https://ozgurtest.azurewebsites.net/api/Sms/Receive

https://docs.microsoft.com/en-us/azure/communication-services/quickstarts/sms/handle-sms-events
