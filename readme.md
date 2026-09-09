# About project

This project represents a relatively simple file server for local file exchange.
**It is not designed for horizontal scaling or exposure to open internet.**

Also, **it serves as example collection** for development of my other projects.
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

The project is designed to be launched from VS Code.
Launch configuration "Start all as watch" is the default thing to use.
It launches asp.net core, angular dev server and browser with code watch both for frontend and backend.

Sometimes you may want to restart backend watch to apply changes like adding attributes or new endpoints.
To do this in "Terminal window" click restart on "dotnet watch" task terminal (in left bar).

## How to debug

To debug the backend use either ".NET Start Debug" or ".NET Core Attach" launch profiles.

**Warning:** due to .NET limitations if watch would apply changes, debugger cannot be connected.
Then just restart backend only (as described in previous chapter) and attach to it.
While debugger is attached, watch cannot apply changes.

For frontend, I was using just `console.log`, LOL.

## Other tasks

Using VS Code "Run task" function you can do more things.
All the custom tasks are prefixed with "SimpleFileServer:".

Tasks that you may be interested in:
- "docker compose up" - Starts the docker compose with our container.
- "docker compose down" - Stops the docker compose with our container.
- "dotnet format" - Auto-formats the backend code to match defined style, run before opening/completing PR.
- "add database migration" - Adds new EF database migration (you would be prompted for name).
- "remove latest database migration" - Removes latest EF database migration.
