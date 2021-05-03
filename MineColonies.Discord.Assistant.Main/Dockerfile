FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MineColonies.Dicord.Assistant/MineColonies.Dicord.Assistant.csproj", "MineColonies.Dicord.Assistant/"]
RUN dotnet restore "MineColonies.Dicord.Assistant/MineColonies.Dicord.Assistant.csproj"
COPY . .
WORKDIR "/src/MineColonies.Dicord.Assistant"
RUN dotnet build "MineColonies.Dicord.Assistant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MineColonies.Dicord.Assistant.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MineColonies.Dicord.Assistant.dll"]
