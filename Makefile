include .env

default: build

## build	:	Build and publish image.
.PHONY: build
build:
	docker-compose build
	docker tag $(shell docker images s21eimages:latest --format="{{.ID}}") erighetto/s21eimages:latest
	docker push erighetto/s21eimages:latest


## run	:	Run with local enviroment.
.PHONY: run
run:
	export $(egrep -v '^#' .env | xargs)
	WD=${PWD}
	cd "${WD}/S21eImages"
	dotnet run --configuration Release -- collect
	wait
	dotnet run --configuration Release -- scrape
	wait
	dotnet run --configuration Release -- export
	wait