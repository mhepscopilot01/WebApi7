#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Base Image uretiyorum. 
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80 443

#Build Image uretiyorum. Sdk image kullanarak build ediyorum.
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["WebApi7.csproj", "."]
RUN dotnet restore "./WebApi7.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./WebApi7.csproj" -c $BUILD_CONFIGURATION -o /app/build

#Publish Image uretiyorum. 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi7.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#Final Image uretiyorum.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 

# Environment Variable tanimliyorum. Cunku uygulama 4500 portunda calisacak.
# burada kaldim. *************
ENV ASPNETCORE_URLS="http://*:4500" 
ENV ASPNETCORE_HTTPS_PORT=443


ENTRYPOINT ["dotnet", "WebApi7.dll"] 