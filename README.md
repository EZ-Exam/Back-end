## Running the Project with Docker

This project provides Docker support for the `teamseven.EzExam.API` ASP.NET Core service. Below are the steps and requirements to build and run the project using Docker Compose.

### Project-Specific Requirements
- **.NET Version:** The Dockerfile uses `.NET 7.0` (set via `ARG DOTNET_VERSION=7.0`).
- **Dependencies:** All dependencies are restored and built as part of the Docker build process. No manual installation is required.

### Environment Variables
- **ASPNETCORE_URLS:** Set to `http://+:80` by default in the Dockerfile. You can override this at runtime if needed.
- **.env File:** If you have a `.env` file in `./teamseven.EzExam.API/`, you can uncomment the `env_file` line in the compose file to load environment variables automatically.

### Build and Run Instructions
1. Ensure Docker and Docker Compose are installed on your system.
2. From the project root, run:
   ```sh
   docker compose up --build
   ```
   This will build the API service and start the container.

### Special Configuration
- The Dockerfile creates a non-root user (`ezexamuser`) for improved security.
- If your API requires a database or other external services, add them as additional services in the `docker-compose.yml` and configure `depends_on` as needed.
- The build process uses cache mounts for NuGet and MSBuild to speed up builds.

### Ports
- **API Service (`csharp-teamseven.ezexam.api`):**
  - Exposes port `80` (mapped to host port `80` by default).

### Networks
- All services are connected to the `ezexam-net` bridge network for inter-service communication.

---
**Note:**
- If you need to customize environment variables, add them to a `.env` file in the API directory and uncomment the `env_file` line in the compose file.
- For additional services (e.g., databases), extend the `docker-compose.yml` accordingly.
