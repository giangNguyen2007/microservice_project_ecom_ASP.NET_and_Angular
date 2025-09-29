FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /

#Restore dependencies

#to be run from solution root folder, not from inside Auth.API folder
COPY SharedLibrary/SharedLibrary.csproj /SharedLibrary/SharedLibrary.csproj

COPY Auth.API/Auth.API.csproj /Auth.API/Auth.API.csproj

RUN dotnet restore /Auth.API/Auth.API.csproj

#Copy code and publish
COPY SharedLibrary/. SharedLibrary/
COPY Auth.API/. Auth.API/

RUN dotnet publish /Auth.API/Auth.API.csproj -c Release -o /Auth.API/publish /p:UseAppHost=false



FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app 

COPY --from=build /Auth.API/publish/. .

COPY Auth.API/Sqlite ./Sqlite

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Auth.API.dll"]