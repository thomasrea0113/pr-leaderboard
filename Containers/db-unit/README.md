# Unit Testing Database

In compose, this is mounted as a temporary file system so that the unit testing database is fresh each time the container is rebuilt.

The postgres docker image automatically runs alls scripts in the `/docker-entrypoint-initdb.d/` directory, so we ensure a script exists
that drops the database defined in `appsettings.unittest.json` each time the container is brought up. This means that we can reuse an
existing database for each session, and be gauranteed a new one next time we open the project. This is useful when rerunning tests, so that
we don't have to drop and reapply all migrations.