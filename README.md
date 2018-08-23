# Notes Demo Application

Public demo: _comming soon_

Local demo: http://localhost:5000

## Local setup

Either build & run it directly from within your favorite IDE or use the following CLI or Docker instructions:

### dotnet CLI

```
$ dotnet run --project NotesApplication/NotesApplication.csproj
```

Tested using dotnet cli version 2.1.4
macOS High Sierra / 10.13.5 (17F77)

### Docker

```
$ docker build -t notes-application .
$ docker run --name notes-application -e ASPNETCORE_ENVIRONMENT=Development --rm -it -p 5000:5000 notes-application
```

Tested using Docker version 18.06.0-ce, build 0ffa825
macOS High Sierra / 10.13.5 (17F77)
