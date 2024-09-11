FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./RailTicketApp/*.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/wait-for-rabbitmq.sh /wait-for-rabbitmq.sh
COPY --from=build /app/publish .
RUN chmod +x /wait-for-rabbitmq.sh

ENTRYPOINT ["/wait-for-rabbitmq.sh", "rabbitmq", "dotnet", "/app/RailTicketApp.dll"]