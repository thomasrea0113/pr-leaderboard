FROM dpage/pgadmin4

ENV PGADMIN_SERVER_JSON_FILE=/var/lib/pgadmin/servers.json
ENV PGPASSFILE=/var/lib/pgadmin/pgpass.conf
ENV PGADMIN_DEFAULT_EMAIL=admin@localhost
ENV PGADMIN_DEFAULT_PASSWORD=admin

COPY ./servers.json /var/lib/pgadmin/servers.json
COPY ./.pgpass /var/lib/pgadmin/pgpass.conf