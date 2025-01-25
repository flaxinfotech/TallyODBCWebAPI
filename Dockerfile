# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file(s) and restore dependencies
COPY TallyIntegrationAPI.csproj ./TallyIntegrationAPI/
WORKDIR /app/TallyIntegrationAPI
RUN dotnet restore

# Copy the rest of the application files and build the app
WORKDIR /app
COPY . .
WORKDIR /app/TallyIntegrationAPI
RUN dotnet publish -c Release -o /app/out

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/out .

# Expose port 80 to the outside world
EXPOSE 80

# Specify the entry point to run the application
ENTRYPOINT ["dotnet", "TallyIntegrationAPI.dll"]
