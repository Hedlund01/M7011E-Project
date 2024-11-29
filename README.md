# Project Setup and Running with Docker Compose

## Prerequisites
- Docker: Make sure Docker is installed on your machine. You can download it from [here](https://www.docker.com/products/docker-desktop).
- Docker Compose: Docker Compose is included with Docker Desktop. Verify the installation by running `docker-compose --version` in your terminal.

## Running the Application

1. **Clone the Repository**

   Clone the repository to your local machine using the following command:

   ```sh
   git clone https://github.com/hedlund01/M7011E-Project.git
   cd M7011E-Project
   ```

2. **Build and Run the Containers**

   Navigate to the directory containing the `docker-compose.yml` file and run the following command to build and start the containers:

   ```sh
   docker compose up --build
   ```

   This command will:
   - Build the Docker images for each service defined in the `docker-compose.yml` file.
   - Start the containers for each service.

3. **Accessing the Services**

   - **RabbitMQ Management Console**: [http://localhost:15672](http://localhost:15672)
   - **Identity API**: [http://localhost:8081](http://localhost:8081)
   - **Catalog API**: [http://localhost:8082](http://localhost:8082)
   - **Ocelot Gateway API**: [http://localhost:8080](http://localhost:8080)

4. **Swagger Documentation**

   Each API has its own Swagger documentation available at the following endpoints:
   - **Identity API Swagger**: [http://localhost:8081/swagger](http://localhost:8081/swagger)
   - **Catalog API Swagger**: [http://localhost:8082/swagger](http://localhost:8082/swagger)
   - **Centralized Swagger Documentation**: [http://localhost:8080/swagger](http://localhost:8080/swagger)

5. **Stopping the Containers**

   To stop the running containers, use the following command:

   ```sh
   docker compose down
   ```

   This command will stop and remove the containers defined in the `docker-compose.yml` file.

## Environment Variables

The `docker-compose.yml` file uses several environment variables to configure the services. Make sure to review and update these variables as needed:

- `DisableEmails`: Set to `true` to disable email sending in the `backgroundservice`.
- `ASPNETCORE_ENVIRONMENT`: Set to `Development` for development environment.
- `ConnectionStrings__DefaultConnection`: Connection string for the databases.

## Volumes

The `docker-compose.yml` file defines named volumes to persist data:

- `rabbitmq_data`: Stores RabbitMQ data.
- `identity_db`: Stores Identity API database data.
- `catalog_db`: Stores Catalog API database data.

These volumes ensure that data is not lost when the containers are stopped or removed.

## Health Checks

The `docker-compose.yml` file includes health checks for the RabbitMQ service to ensure it is running correctly before starting dependent services.

## Additional Information

For more details on Docker and Docker Compose, refer to the official documentation:

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)