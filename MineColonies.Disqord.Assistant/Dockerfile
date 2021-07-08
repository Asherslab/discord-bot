FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["MineColonies.Disqord.Assistant/MineColonies.Disqord.Assistant.csproj", "MineColonies.Disqord.Assistant/"]
RUN dotnet restore "MineColonies.Disqord.Assistant/MineColonies.Disqord.Assistant.csproj"
COPY . .
WORKDIR "/src/MineColonies.Disqord.Assistant"
RUN dotnet build "MineColonies.Disqord.Assistant.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MineColonies.Disqord.Assistant.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MineColonies.Disqord.Assistant.dll"]
