FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY ./publish/ ./

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080


ENTRYPOINT ["dotnet", "GateWay.API.dll"]

