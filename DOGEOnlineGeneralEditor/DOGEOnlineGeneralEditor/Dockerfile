# The `FROM` instruction specifies the base image. You are
# extending the `microsoft/aspnet` image.

FROM microsoft/aspnet

# Open the port
# EXPOSE 80

# The final instruction copies the site you published earlier into the container.
COPY ./bin/Release/PublishOutput/ /inetpub/wwwroot



# Commands:
#
# docker build -t dogemvc .
#
# docker run -d --name doge dogemvc
# -p 8080:8080



# Find IP address of local docker image
# docker inspect -f "{{ .NetworkSettings.Networks.nat.IPAddress }}" doge
#
# See all images
# docker images