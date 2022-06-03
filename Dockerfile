FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY *.sln .

COPY ["./Badaboom.Backend/Badaboom.Backend.csproj", "Badaboom.Backend/"]
COPY ["./Badaboom.Backend.Infrastructure/Badaboom.Backend.Infrastructure.csproj", "Badaboom.Backend.Infrastructure/"]
COPY ["./Badaboom.Client/Badaboom.Client.csproj", "Badaboom.Client/"]
COPY ["./Badaboom.Client.Infrastructure/Badaboom.Client.Infrastructure.csproj", "Badaboom.Client.Infrastructure/"]
COPY ["./Badaboom.Core/Badaboom.Core.csproj", "Badaboom.Core/"]
COPY ["./Indexer/Indexer.csproj", "Indexer/"]
COPY ["./Indexing.Core/Indexer.Core.csproj", "Indexing.Core/"]
COPY ["./Database/Database.csproj", "Database/"]
COPY ["./Monitor/Monitor.csproj", "Monitor/"]

RUN dotnet restore 
COPY . .

WORKDIR /src

WORKDIR "/src/Indexer"

RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS indexer
WORKDIR /app
COPY --from=publish /app/publish .

ARG DB_CONNECTION_STRING=server="(LocalDb)\MSSQLLocalDB;Initial Catalog=indexerBadaboomDb;Integrated Security=True;MultipleActiveResultSets=True;"
ARG LOG_FOLDER="/app/logs/log.log"
ARG LOG_CRITICAL_FOLDER="/app/logs/log.critical.log"
ARG RPC_URL="http://localhost:8545"
ARG INDEX_INNER_CALLS="false"
ARG BLOCK_QUEUE_SIZE=100
ARG INDEX_START_BLOCK=0


ENV DB_CONNECTION_STRING=${DB_CONNECTION_STRING}
ENV LOG_FOLDER=${LOG_FOLDER}
ENV LOG_CRITICAL_FOLDER=${LOG_CRITICAL_FOLDER}
ENV RPC_URL=${RPC_URL}
ENV INDEX_INNER_CALLS=${INDEX_INNER_CALLS}
ENV BLOCK_QUEUE_SIZE=${BLOCK_QUEUE_SIZE}
ENV INDEX_START_BLOCK=${INDEX_START_BLOCK}

RUN echo "ConnString ${DB_CONNECTION_STRING}" 
RUN echo "LOG_FOLDER ${LOG_FOLDER}"
RUN echo "RPC_URL ${RPC_URL}"
RUN echo "INDEX_INNER_CALLS ${INDEX_INNER_CALLS}"

RUN echo $DB_CONNECTION_STRING

ENV ConnectionStrings:CrimeChain $DB_CONNECTION_STRING

ENTRYPOINT ["dotnet", "Indexer.dll"]
