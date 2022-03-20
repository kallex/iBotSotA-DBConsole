#!/bin/sh

/usr/bin/xray-daemon -f /var/log/xray-daemon.log &
cd /app
dotnet iBotSotALambda.dll