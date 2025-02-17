FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/OrderService/OrderService.Core/OrderService.Core.csproj", "OrderService.Core/"]
COPY ["src/OrderService/OrderService.CQRS/OrderService.CQRS.csproj", "OrderService.CQRS/"]
COPY ["src/OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
COPY ["src/OrderService/OrderService.Entities/OrderService.Entities.csproj", "OrderService.Entities/"]
COPY ["src/OrderService/OrderService.Mapping/OrderService.Mapping.csproj", "OrderService.Mapping/"]
COPY ["src/OrderService/OrderService.Repositories/OrderService.Repositories.csproj", "OrderService.Repositories/"]
COPY ["src/OrderService/OrderService.Validation/OrderService.Validation.csproj", "OrderService.Validation/"]
COPY ["src/OrderService/OrderService.OutboxDaemon/OrderService.OutboxDaemon.csproj", "OrderService.OutboxDaemon/"]
COPY ["src/OrderService/OrderService.Postgres/OrderService.Postgres.csproj", "OrderService.Postgres/"]
COPY ["src/OrderService/OrderService.gRPC/OrderService.gRPC.csproj", "OrderService.gRPC/"]
RUN dotnet restore "OrderService.Core/OrderService.Core.csproj"
COPY . .
WORKDIR "/src/OrderService/OrderService.Core"
RUN dotnet build "OrderService.Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Core.dll"]
