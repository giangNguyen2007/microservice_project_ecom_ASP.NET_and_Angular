FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

#Restore dependencies

#to be run from solution root folder


COPY GateWay.API/GateWay.API.csproj GateWay.API/GateWay.API.csproj

RUN dotnet restore GateWay.API/GateWay.API.csproj


COPY GateWay.API/. GateWay.API/

RUN dotnet publish GateWay.API/GateWay.API.csproj -c Release -o GateWay.API/publish /p:UseAppHost=false



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /GateWay.API/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080


ENTRYPOINT ["dotnet", "GateWay.API.dll"]

