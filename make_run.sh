#!/usr/bin/env bash

## Run with local enviroment.
export $(egrep -v '^#' .env | xargs)
cd "$(PWD)/S21eImages"
dotnet run --configuration Release -- collect all
wait
dotnet run --configuration Release -- scrape
wait
dotnet run --configuration Release -- export
wait