FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY src/API.csproj .
RUN dotnet restore API.csproj
COPY src/. .
RUN rm global.json
RUN dotnet build API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "API.dll"]