FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProjectService/ProjectService.csproj", "ProjectService/"]
COPY ["SharedPagination/SharedPagination.csproj", "SharedPagination/"]
RUN dotnet restore "ProjectService/ProjectService.csproj"
COPY . .
WORKDIR "/src/ProjectService"
RUN dotnet build "ProjectService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectService.dll"]