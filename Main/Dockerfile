FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet nuget locals all --clear
RUN dotnet restore

COPY . ./
RUN dotnet publish -c release -o /dist --no-restore --no-cache

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /dist
COPY --from=build /dist .
ENTRYPOINT ["dotnet", "Main.dll", "--server.urls", "http://*:5000"]
