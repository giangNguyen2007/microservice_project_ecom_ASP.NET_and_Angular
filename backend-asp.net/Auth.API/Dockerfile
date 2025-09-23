
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY ./publish/ ./

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

RUN mkdir /app/Sqlite

ENTRYPOINT ["dotnet", "Auth.API.dll"]