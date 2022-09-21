FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY ./Acs.Messaging.Sample/Server/bin/Release/net6.0 /app
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Acs.Messaging.Sample.Server.dll"]
