#!/usr/bin/env bash

export $(egrep -v '^#' .env | xargs)

WD=${PWD}

cd "${WD}/S21eImages"

dotnet run --configuration Release -- collect
wait

dotnet run --configuration Release -- scrape
wait

dotnet run --configuration Release -- export
wait