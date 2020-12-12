#!/usr/bin/env bash

while [[ "$#" -gt 0 ]]; do
    case $1 in
        -t|--target) target="$2"; shift ;;
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

echo "Run with $target enviroment"

## Run with local enviroment.
if [ "$target" == "local" ]
then
    export $(egrep -v '^#' .env | xargs)
    cd "$(PWD)/S21eImages"
    dotnet ef database update
    wait
    dotnet run --configuration Release -- collect 60
    wait
    dotnet run --configuration Release -- scrape
    wait
    dotnet run --configuration Release -- export
    wait
fi

## Run with container enviroment.
if [ "$target" == "container" ]
then
    docker-compose build
    docker-compose up
fi
