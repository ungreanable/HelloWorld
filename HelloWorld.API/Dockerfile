#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
RUN apk add --no-cache tzdata
ENV TZ=Asia/Bangkok
ARG BUILD_VERSION
ENV BUILD_VERSION=$BUILD_VERSION
USER app
WORKDIR /app
RUN mkdir Response
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HelloWorld.API/HelloWorld.API.csproj", "HelloWorld.API/"]
RUN dotnet restore "./HelloWorld.API/./HelloWorld.API.csproj"
COPY . .
WORKDIR "/src/HelloWorld.API"
RUN dotnet build "./HelloWorld.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HelloWorld.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN chmod 0777 Response/response.json
ENV TZ=Asia/Bangkok
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HelloWorld.API.dll"]