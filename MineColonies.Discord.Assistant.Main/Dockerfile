FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MineColonies.Discord.Assistant.Main/MineColonies.Discord.Assistant.Main.csproj", "MineColonies.Discord.Assistant.Main/"]
RUN dotnet restore "MineColonies.Discord.Assistant.Main/MineColonies.Discord.Assistant.Main.csproj"
COPY . .
WORKDIR "/src/MineColonies.Discord.Assistant.Main"
RUN dotnet build "MineColonies.Discord.Assistant.Main.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MineColonies.Discord.Assistant.Main.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MineColonies.Discord.Assistant.Main.dll"]
