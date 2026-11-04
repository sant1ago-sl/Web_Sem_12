FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY api/*.csproj api/
RUN dotnet restore api/TallerMecanico.Api.csproj
COPY api/ api/
COPY scripts/ scripts/
RUN dotnet publish api/TallerMecanico.Api.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080 DISABLE_HTTPS_REDIRECT=true
EXPOSE 8080
ENTRYPOINT ["dotnet", "TallerMecanico.Api.dll"]
