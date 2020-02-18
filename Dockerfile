FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic

RUN wget -q -O - https://deb.nodesource.com/setup_13.x | bash

RUN apt update && apt upgrade -y && \
    bash -c "debconf-set-selections <<< 'postfix postfix/mailname string pr.com'" && \
    bash -c "debconf-set-selections <<< 'postfix postfix/main_mailer_type string '\''Internet Site'\'''" && \
    apt install -y \
    nodejs \
    #
    # Other general depencencies
    iproute2 net-tools mailutils \
    #
    # Install Docker CE CLI
    apt-transport-https ca-certificates curl gnupg-agent software-properties-common lsb-release \
    && curl -fsSL https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/gpg | apt-key add - 2>/dev/null \
    && add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]') $(lsb_release -cs) stable" \
    && apt-get update \
    && apt-get install -y docker-ce-cli \
    #
    # Install Docker Compose
    && curl -sSL "https://github.com/docker/compose/releases/download/1.24.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose \
    && chmod +x /usr/local/bin/docker-compose

WORKDIR /app/ClientApp

ARG BUST=45

COPY ./ClientApp/package.json .
RUN BUST=${BUST} npm install

# copy all workspace files to the /app directory
COPY . ../

WORKDIR /app/Leaderboard

RUN dotnet tool restore

# RUN dotnet build /p:BuildClient=true

CMD [ "sh", "-c", "postfix start && dotnet run 0.0.0.0:5000" ]