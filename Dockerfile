FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app
COPY . ./

RUN dotnet restore NotesApplication.sln
RUN dotnet publish NotesApplication/NotesApplication.csproj -c Release -o /app/out
RUN ls -lha /app/out
# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
RUN mkdir /app/out
COPY --from=build-env /app/out .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://*:5000
ENTRYPOINT ["dotnet", "NotesApplication.dll"]

