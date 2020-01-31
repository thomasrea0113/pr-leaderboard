FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic

RUN wget -q -O - https://deb.nodesource.com/setup_13.x | bash

RUN apt update && apt upgrade -y && apt install -y \
 nodejs

WORKDIR /app/ClientApp

ARG BUST=45

COPY ./ClientApp/package.json .
RUN BUST=${BUST} npm install

COPY ./ClientApp .

WORKDIR /app/Leaderboard

COPY ./Leaderboard .
COPY ./Leaderboard.Tests ../Leaderboard.Tests

RUN dotnet build /p:BuildClient=true

CMD [ "sh", "-c", "dotnet run 0.0.0.0:5000" ]