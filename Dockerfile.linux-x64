FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY GenHTTP.Gateway/*.csproj ./gateway/
WORKDIR /app/gateway
RUN dotnet restore

# copy and build app and libraries
WORKDIR /app/
COPY GenHTTP.Gateway/. ./gateway/
WORKDIR /app/gateway
RUN dotnet publish -c Release -r linux-musl-x64 -o out

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/gateway/out ./
ENTRYPOINT ["./GenHTTP.Gateway"]

VOLUME ["/app/data", "/app/certs", "/app/config"]

EXPOSE 80 443
