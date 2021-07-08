FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MineColonies.Discord.Assistant.Remora/MineColonies.Discord.Assistant.Remora.csproj", "MineColonies.Discord.Assistant.Remora/"]
RUN dotnet restore "MineColonies.Discord.Assistant.Remora/MineColonies.Discord.Assistant.Remora.csproj"
COPY . .
WORKDIR "/src/MineColonies.Discord.Assistant.Remora"
RUN dotnet build "MineColonies.Discord.Assistant.Remora.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MineColonies.Discord.Assistant.Remora.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MineColonies.Discord.Assistant.Remora.dll"]
