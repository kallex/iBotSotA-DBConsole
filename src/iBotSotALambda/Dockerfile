#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:3.1

COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
COPY xray /usr/bin/xray-daemon
COPY . app/
WORKDIR /app
#ENTRYPOINT ["dotnet", "iBotSotALambda.dll"]
ENTRYPOINT ["/entrypoint.sh"]