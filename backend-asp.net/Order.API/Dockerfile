FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

#Restore dependencies
#to be run from solution root folder
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/SharedLibrary.csproj

COPY Order.API/Order.API.csproj Order.API/Order.API.csproj

RUN dotnet restore Order.API/Order.API.csproj

#Copy code and publish
COPY SharedLibrary/. SharedLibrary/
COPY Order.API/. Order.API/

RUN dotnet publish Order.API/Order.API.csproj -c Release -o Order.API/publish /p:UseAppHost=false


# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /Order.API/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080


ENTRYPOINT ["dotnet", "Order.API.dll"]
