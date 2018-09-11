# Notes Demo Application

Demo: https://notes-application.sushee.ch/

Ready-to-use Docker-Image: https://hub.docker.com/r/chrigu/notes-application-mvc-example/

## Setup instructions

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


## Running Tests

```
dotnet test -t NotesApplication.Tests/NotesApplication.Tests.csproj
```

### Controller Tests
![Controller Tests](https://raw.githubusercontent.com/chrigu-ebert/notes-application-mvc-example/master/controller-tests.png)

### Service Tests
![Service Tests](https://raw.githubusercontent.com/chrigu-ebert/notes-application-mvc-example/master/service-tests.png)
