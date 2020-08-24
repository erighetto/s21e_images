#!/usr/bin/env bash

export $(egrep -v '^#' .env | xargs)

WD=${PWD}

cd "${WD}/src/ImagesCollect"
dotnet run --configuration Release
wait

cd "${WD}/src/ImagesScrape"
dotnet run --configuration Release
wait

cd "${WD}/src/ImagesExport"
dotnet run --configuration Release
wait