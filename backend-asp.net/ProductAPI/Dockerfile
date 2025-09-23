
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY ./publish/ ./

COPY ./cert.pfx ./cert.pfx

EXPOSE 8080

# in containers, use HTTP unless you configure certs
ENV ASPNETCORE_URLS=http://+:5000
# Use HTTPS in container with self-signed certificate
#ENV ASPNETCORE_Kestrel__Certificates__Default__Password="certs_password"
#ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/cert.pfx

ENTRYPOINT ["dotnet", "ProductAPI.dll"]
