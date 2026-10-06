FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5110

ENV ASPNETCORE_URLS=http://+:5110

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/GameReserve.WebApi/GameReserve.WebApi.csproj", "src/GameReserve.WebApi/"]
RUN dotnet restore "src/GameReserve.WebApi/GameReserve.WebApi.csproj"
COPY . .
WORKDIR "/src/src/GameReserve.WebApi"
RUN dotnet build "GameReserve.WebApi.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "GameReserve.WebApi.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameReserve.WebApi.dll"]
