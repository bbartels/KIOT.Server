FROM microsoft/dotnet:3.0-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:3.0-sdk AS build
WORKDIR /src
COPY ["KIOT.Server.Api/KIOT.Server.Api.csproj", "KIOT.Server.Api/"]
RUN dotnet restore "KIOT.Server.Api/KIOT.Server.Api.csproj"
COPY . .
WORKDIR "/src/KIOT.Server.Api"
RUN dotnet build "KIOT.Server.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "KIOT.Server.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KIOT.Server.Api.dll"]