FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV PROTOBUF_PROTOC=/usr/bin/protoc
ENV gRPC_PluginFullPath=/usr/bin/grpc_csharp_plugin
RUN apk add protobuf protobuf-dev grpc grpc-plugins
WORKDIR /src
COPY ["Services/PaymentApi/PaymentApi.csproj", "Services/PaymentApi/"]
COPY ["BuildingBlocks/Shared/Shared.csproj", "BuildingBlocks/Shared/"]

RUN dotnet restore "Services/PaymentApi/PaymentApi.csproj"
COPY . .
WORKDIR "/src/Services/PaymentApi"
RUN dotnet build "PaymentApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentApi.dll"]