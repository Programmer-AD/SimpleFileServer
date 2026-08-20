ARG _DOTNET_VERSION=10.0

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:${_DOTNET_VERSION}-alpine AS build

# For readability only
ARG _PROJECT_PATH=./backend/src/SimpleFileServer.Web

# The context is repository root
COPY . /source
WORKDIR /source

ARG BUILD_CONFIGURATION=Release

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish ${_PROJECT_PATH} -c ${BUILD_CONFIGURATION} -p:UseAppHost=false -o /app

FROM mcr.microsoft.com/dotnet/aspnet:${_DOTNET_VERSION}-alpine AS final

COPY --from=build /app /app
WORKDIR /app

# Switch to non-privileged user (defined in dotnet base image)
USER $APP_UID

# Set exposed port
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80

ENTRYPOINT [ "dotnet", "SimpleFileServer.Web.dll" ]
