# Introduction

The GenHTTP Gateway provides a simple way to provide all your web applications over a single, HTTPS secured entry point.

# Tags

| Tag           | Description |
| ------------- |-------------|
| linux-x64     | Alpine based image to run on Linux x64 hosts |

# Initial Setup

When starting the gateway via docker, a example configuration file will be created in the mounted
configuration directory. Adjust this configuration file to your need and restart the container.

~~~bash
sudo docker run -d -p 80:80 \
           -p 443:443 \
		   -v /data/gateway/config:/app/config \
		   -v /data/gateway/data:/app/data \
		   -v /data/gateway/certs:/app/certs \
		   genhttp/gateway:linux-x64
~~~

Sample configuration file:

~~~yaml
hosts:

  # configuration for a specific host
  # duplicate this entry to add another host
  domain1.com:

    # uncomment this block to support SSL
    # mount your certificates in /app/certs

    # security:
    #   certificate:
    #    pfx: domain1.pfx

    # the web page that will be shown if domain1.com is called
    default:
      destination: http://10.0.0.2:8080

    # additional routes to be accessible via the gateway
    routes:

      # routes can be chained, the children of this entry will be
      # accessible via domain1.com/admin/
      admin:

        routes:

          # domain1.com/admin/portainer/
          portainer:
            destination: http://10.0.0.2:9000

          # domain1.com/admin/pi-hole/
          pi-hole:
            destination: http://10.0.0.3/admin/
~~~

# Volumes

| Volume        | Description |
| ------------- |-------------|
| /data/gateway/config | The configuration files of the gateway |
| /data/gateway/certs | The certificates to be used for SSL |
| /data/gateway/data | Additional data such as the .well-known folder |

# SSL / Let's Encrypt

The gateway can be used with `certbot` to generate SSL certificates:

~~~bash
certbot certonly --webroot -w /data/gateway/data/ -d domain1.com
~~~

Currently, only PFX certificates are supported by the gateway. Run the following command to convert the certificates generated by `certbot` and copy the resulting file into the mounted certificate folder.

~~~bash
openssl pkcs12 -export -out domain1.com.pfx -inkey privkey.pem -in fullchain.pem
~~~