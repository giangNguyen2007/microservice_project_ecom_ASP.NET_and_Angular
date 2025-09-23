
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

#Restore dependencies

#to be run from solution root folder, not from inside Payment.API folder
COPY SharedLibrary/SharedLibrary.csproj /SharedLibrary/SharedLibrary.csproj

COPY Payment.API/Payment.API.csproj /Payment.API/Payment.API.csproj

RUN dotnet restore /Payment.API/Payment.API.csproj

#Copy code and publish
COPY SharedLibrary/. SharedLibrary/
COPY Payment.API/. Payment.API/

RUN dotnet publish /Payment.API/Payment.API.csproj -c Release -o /Payment.API/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
EXPOSE 8080

WORKDIR /app
COPY --from=build /Payment.API/publish .

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Payment.API.dll"]