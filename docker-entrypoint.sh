#!/usr/bin/env bash

export DISPLAY=:20
Xvfb :20 -screen 0 1366x768x16 &

cd /app

dotnet S21eImages.dll collect
wait

dotnet S21eImages.dll scrape
wait

dotnet S21eImages.dll scrape
wait