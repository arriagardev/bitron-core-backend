FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BitronCore.Domain/BitronCore.Domain.csproj", "src/BitronCore.Domain/"]
COPY ["src/BitronCore.Application/BitronCore.Application.csproj", "src/BitronCore.Application/"]
COPY ["src/BitronCore.Infrastructure/BitronCore.Infrastructure.csproj", "src/BitronCore.Infrastructure/"]
COPY ["src/BitronCore.Api/BitronCore.Api.csproj", "src/BitronCore.Api/"]

RUN dotnet restore "src/BitronCore.Api/BitronCore.Api.csproj"

COPY . .
RUN dotnet publish "src/BitronCore.Api/BitronCore.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BitronCore.Api.dll"]
