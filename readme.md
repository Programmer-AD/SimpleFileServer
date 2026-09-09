# About project

This project represent a relative simple file server for local file exchange.
It is not designed for horizontal scaling or exposure to open internet.

It serves as example collection for development of my other projects.
That's why some things may look overcomplicated.


# Technologies

## Backend

- .NET Core 10
- ASP.NET Core 10
- EntityFramework Core 10 (with SQLite driver)
- XUnit test framework
- Moq mocking library

## Frontend

- Angular 22
- TypeScript
- SCSS

## Other

- SQLite
- Docker
- Docker Compose
- VS Code task setup


# How to work with it

## How to launch

The project is intended to be launched from VS Code.
Lauch configuration "Start all as watch" is the default thing to use.
It launches asp.net core, angular dev server and browser with code watch both for frontend and backend.

Sometimes you may want to restart backend watch to apply changes like adding attributes or new endpoints.
To do this in "Terminal window" click restart on "dotnet watch" task terminal (in left bar).

## How to debug

To debug the backend use either ".NET Start Debug" or ".NET Core Attach" launch profiles.

**Warning:** due to .NET limitations if watch would apply changes, debugger cannot be connected.
hen just restart backend only (as described in previous chapter) and attach to it.
While debugger is attached, watch cannot apply changes.

For frontend, I was using just `console.log`, LOL.

## Other tasks

Using VS Code "Run task" function you do more things.
All my custom tasks are prefixed with "SimpleFileServer: ".

Tasks that you may be interested in:
- "docker compose up" - Starts the docker compose with our container.
- "docker compose down" - Stops the docker compose with our container.
- "dotnet format" - Autoformats the backend code to match defined style.
- "add database migration" - Adds new EF database migration (you would be prompted for name).
- "remove latest database migration" - Removes latest EF database migration.
