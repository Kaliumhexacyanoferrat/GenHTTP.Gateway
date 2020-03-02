#!/bin/bash

docker manifest create genhttp/gateway:latest genhttp/gateway:linux-x64 genhttp/gateway:linux-arm32 genhttp/gateway:linux-arm64 genhttp/gateway:windows-x64 genhttp/gateway:windows-arm32
docker manifest push --purge genhttp/gateway:latest