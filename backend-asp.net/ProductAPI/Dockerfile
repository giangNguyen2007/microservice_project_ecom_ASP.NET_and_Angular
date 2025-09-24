FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

#Restore dependencies

#to be run from solution root folder
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/SharedLibrary.csproj

COPY ProductAPI/ProductAPI.csproj ProductAPI/ProductAPI.csproj

RUN dotnet restore ProductAPI/ProductAPI.csproj

#Copy code and publish
COPY SharedLibrary/. SharedLibrary/
COPY ProductAPI/. ProductAPI/

RUN dotnet publish ProductAPI/ProductAPI.csproj -c Release -o ProductAPI/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=build /ProductAPI/publish .

COPY ProductAPI/cert.pfx ./cert.pfx

COPY ProductAPI/Sqlite ./Sqlite

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:5000
# Use HTTPS in container with self-signed certificate
#ENV ASPNETCORE_Kestrel__Certificates__Default__Password="certs_password"
#ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert.pfx

ENTRYPOINT ["dotnet", "ProductAPI.dll"]
