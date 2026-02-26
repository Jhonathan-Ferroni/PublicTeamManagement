# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia apenas o arquivo de projeto primeiro (melhora o cache do Docker)
# Como seu projeto está dentro da pasta StarChampionship, o caminho é esse:
COPY StarChampionship/StarChampionship.csproj StarChampionship/

# 2. Executa o restore diretamente no arquivo de projeto
RUN dotnet restore StarChampionship/StarChampionship.csproj

# 3. Agora copia todo o resto dos arquivos
COPY . .

# 4. Entra na pasta do projeto para buildar e publicar
WORKDIR "/src/StarChampionship"
RUN dotnet publish "StarChampionship.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Configuração de porta para o Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "StarChampionship.dll"]