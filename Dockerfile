# Usa a imagem do SDK do .NET para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# 1. Copia o arquivo de projeto e restaura dependências
# Usamos o curinga para pegar qualquer .csproj na raiz
COPY *.csproj ./
RUN dotnet restore

# 2. Copia todo o resto e compila
COPY . .
RUN dotnet publish -c Release -o out

# 3. Imagem final (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
# Copia o resultado do 'out' da etapa anterior
COPY --from=build /app/out .

# Comando para iniciar a aplicação
# Certifique-se que o nome da DLL é exatamente esse
ENTRYPOINT ["dotnet", "GamingJourney.dll"]