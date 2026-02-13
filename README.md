# MongoDb.Playground

[![CI](https://github.com/M3LiNdRu/MongoDb.Playground/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/M3LiNdRu/MongoDb.Playground/actions/workflows/ci.yml)

MongoLab - Play, Test and have fun!

## Services

This Docker Compose Setup provides a MongoDB environment with MongoDB instance, mongo-express web interface and a tools container for command-line operations.

### mongo replicaset
- **Image**: `mongo:latest`
- **Port**: `27017`
- **Volume**: `mongo_data` mounted at `/data/db`

### mongo-express
- **Image**: `mongo-express:latest`
- **Port**: `8081`
- **URL**: http://localhost:8081
- Web-based MongoDB admin interface with no basic auth required

### mongo-tools
- **Image**: `mongo:latest`
- Utility container with mongosh, mongodump, and mongorestore
- Keeps running with `sleep infinity` for interactive access
- **Volume**: `./data` directory mounted at `/backups`


## Usage

### Start the Services
```bash
docker compose up -d
```

### Stop the Services
```bash
docker compose down
```

### Access mongo-express
Open your browser and navigate to:
```
http://localhost:8081
```

### Using mongo-tools Container

#### Connect to the Container
```bash
docker exec -it mongodbplayground-mongo-tools-1 bash

# Connect using connection string
mongosh mongodb://mongo:27017/?directConnection=true
```

#### Using mongodump & mongorestore
https://stackoverflow.com/questions/4880874/how-do-i-create-a-mongodb-dump-of-my-database

#### Using mongoimport
```bash
mongoimport --uri="mongodb://mongo:27017/?directConnection=true" --db MovieStore --collection movies --file /backups/Movies/Movies.json
```

#### Using mongodump & mongorestore
```bash
# Dump a specific database and collection to the backups volume
mongodump --uri="mongodb://mongo:27017/?directConnection=true" --db MovieStore --collection movies --out=/backups/2026/02/13

mongoexport --uri="mongodb://mongo:27017/?directConnection=true" --collection=roger-query --out=emailInfoTable.json --jsonArray

# Restore a database from the backups volume
mongorestore --uri="mongodb://mongo:27017/?directConnection=true" --db MovieStore --collection movies --archive --gzip /backups/MovieStore-2026-02-08.archive.gz
```

#### Running JavaScript Scripts

You can execute JavaScript files directly against the database using mongosh:

```bash
# Create a script file in the backups directory (accessible from host at ./scripts)
# Example: ./scripts/add-property.js

# Run the script from within the mongo-tools container
mongosh mongodb://mongo:27017/?directConnection=true&authSource=admin /scripts/add-property.js
```
**Alternative: Run inline JavaScript**
```bash
# Execute JavaScript directly without a file
mongosh mongodb://mongo:27017/?directConnection=true&authSource=admin --eval "db.movies.updateMany({}, {\$set: {newProperty: 'value'}})"
```

## Volumes
- `mongodb_data`: Persists MongoDB data across container restarts
- `./data`: Local directory for backup files (directly accessible from your PC)
- `./scripts`: Local directory for script files (directly accessible from your PC)

#### VS Code MongoDB MCP server

This repo includes a VS Code MCP server definition in `.vscode/mcp.json`.

- Does **not** require Node.js installed locally (it runs `mongodb-mcp-server` inside a Docker container)
- Requires `docker` (and access to the Docker daemon)
- Default connection string is `mongodb://localhost:27017` (works with the provided `docker-compose.yaml` on Linux via host networking)

