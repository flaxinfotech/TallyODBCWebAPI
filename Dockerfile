# Use the official .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file(s) and restore dependencies
COPY TallyIntegrationAPI.csproj ./
RUN dotnet restore

# Copy the rest of the application files and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official .NET 8.0 runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/out .

# Expose port 80 to the outside world
EXPOSE 80

# Define the entry point for the application
ENTRYPOINT ["dotnet", "TallyIntegrationAPI.dll"]
