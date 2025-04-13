# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar os arquivos de projeto e restaurar dependências
COPY ./api-upload/api-upload.csproj ./api-upload/
RUN dotnet restore ./api-upload/api-upload.csproj

# Copiar o restante dos arquivos e build
COPY . . 
WORKDIR /src/api-upload
RUN dotnet build -c Release -o /app/build

# Publicar a aplicação
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish . 
EXPOSE 5000
ENTRYPOINT ["dotnet", "api-upload.dll"]
