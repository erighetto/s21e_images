#!/usr/bin/env bash

## Build and publish image.
export $(egrep -v '^#' .env | xargs)
docker-compose build
docker push ${DOCKER_REGISTRY}s21eimages:latest