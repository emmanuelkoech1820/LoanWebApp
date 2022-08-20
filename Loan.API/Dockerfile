#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Loan.API/Loan.API.csproj", "Loan.API/"]
COPY ["Loan.Core/Loan.Core.csproj", "Loan.Core/"]
COPY ["Loan.Data/Loan.Data.csproj", "Loan.Data/"]
RUN dotnet restore "Loan.API/Loan.API.csproj"
COPY . .
WORKDIR "/src/Loan.API"
RUN dotnet build "Loan.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Loan.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Loan.API.dll"]
