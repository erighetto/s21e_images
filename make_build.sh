#!/usr/bin/env bash

## Build and publish image.
docker-compose build
docker tag $(docker images s21eimages:latest --format="{{.ID}}") erighetto/s21eimages:latest
docker push erighetto/s21eimages:latest