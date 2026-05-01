FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Portfolio.Project/Portfolio.Project.csproj", "Portfolio.Project/"]
COPY ["SharedPagination/SharedPagination.csproj", "SharedPagination/"]
RUN dotnet restore "Portfolio.Project/Portfolio.Project.csproj"
COPY . .
WORKDIR "/src/Portfolio.Project"
RUN dotnet build "Portfolio.Project.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Portfolio.Project.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Portfolio.Project.dll"]
