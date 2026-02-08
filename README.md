# MongoDb.Playground

[![CI](https://github.com/M3LiNdRu/MongoDb.Playground/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/M3LiNdRu/MongoDb.Playground/actions/workflows/ci.yml)

MongoLab - Play, Test and have fun!

## VS Code MCP (MongoDB)

This repo includes a VS Code MCP server definition in `.vscode/mcp.json`.

- Does **not** require Node.js installed locally (it runs `mongodb-mcp-server` inside a Docker container)
- Requires `docker` (and access to the Docker daemon)
- Default connection string is `mongodb://localhost:27017` (works with the provided `docker-compose.yaml` on Linux via host networking)
