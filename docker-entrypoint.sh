#!/usr/bin/env bash

export DISPLAY=:20 DBUS_SESSION_BUS_ADDRESS=/dev/null

RANGE=${RANGE:-60}

if [ -f "/tmp/.X20-lock" ]; then
    rm /tmp/.X20-lock
fi

Xvfb :20 -screen 0 1366x768x16 -nolisten tcp -nolisten unix &

cd /app

dotnet S21eImages.dll collect ${RANGE}

dotnet S21eImages.dll scrape

dotnet S21eImages.dll export