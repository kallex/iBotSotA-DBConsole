#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:3.1
COPY xray /usr/bin/xray-daemon
CMD xray-daemon -f /var/log/xray-daemon.log &
COPY . app/
WORKDIR /app
ENTRYPOINT ["dotnet", "iBotSotALambda.dll"]