# GenHTTP Gateway

The GenHTTP Gateway provides a simple way to serve all your web applications over a single, HTTPS secured entry point. Compared to other reverse-proxy solutions such as [Traefik](https://github.com/containous/traefik), the gateway provides less features but is easier to configure for scenarios such as home servers.

![CI](https://github.com/Kaliumhexacyanoferrat/GenHTTP.Gateway/workflows/CI/badge.svg) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=GenHTTP.Gateway&metric=coverage)](https://sonarcloud.io/dashboard?id=GenHTTP.Gateway)

## Usage

The GenHTTP Gateway is available as a docker image on [DockerHub](https://hub.docker.com/r/genhttp/gateway). You will find examples on how to run and to configure the gateway there.

## Development

Building the gateway from source requires the [.NET Core SDK](https://dotnet.microsoft.com/download) to be installed.
The following commands will clone the repository and run the gateway on port 80:

~~~bash
git clone https://github.com/Kaliumhexacyanoferrat/GenHTTP.Gateway.git
cd GenHTTP.Gateway
dotnet run
~~~

As the default configuration uses `domain1.com` as an example, you may want to add the following entry to your `/etc/hosts` file (or `C:\Windows\System32\drivers\etc\hosts` on Windows):

~~~bash
127.0.0.1 domain1.com
~~~

You should then be able to open http://domain1.com/directory-browsing/ in your browser.

## Building Docker Images

To build a docker image, run

~~~bash
docker build -f Dockerfile.linux-x64 -t genhttp/gateway:linux-x64 .
~~~

To build an image for a different platform simply select a different platform name.
