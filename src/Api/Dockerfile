# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o csproj primeiro para aproveitar cache do Docker
COPY Api/Api.csproj Api/
COPY Application/Application.csproj Application/
COPY Infra/Infra.csproj Infra/
RUN dotnet restore Api/Api.csproj

# Copia o resto do código
COPY . .
RUN dotnet publish -c Release -o /out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]
