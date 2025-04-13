# Base image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto e restaurar dependências
COPY ./console-consumer/console-consumer.csproj ./console-consumer/
RUN dotnet restore ./console-consumer/console-consumer.csproj

# Copiar o restante dos arquivos e build
COPY . . 
WORKDIR /src/console-consumer
RUN dotnet build -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
ENTRYPOINT ["dotnet", "console-consumer.dll"]
