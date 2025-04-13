# Etapa 1: Imagem base para execução
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Etapa 2: Imagem de build para construção e publicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar o arquivo do projeto e restaurar as dependências
COPY ./console-consumer/console-consumer.csproj ./console-consumer/
RUN dotnet restore ./console-consumer/console-consumer.csproj

# Copiar o restante dos arquivos e compilar
COPY . . 
WORKDIR /src/console-consumer
RUN dotnet build -c Release -o /app/build

# Etapa 3: Publicação da aplicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Etapa 4: Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 

# Definindo a execução do aplicativo console
ENTRYPOINT ["dotnet", "console-consumer.dll"]
