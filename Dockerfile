# Use official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY *.sln ./
COPY src/TravelsBE/TravelsBE.csproj ./TravelsBE/


# Copy the rest of the application files
COPY . ./

# Restore dependencies
RUN dotnet restore

# Build the application in Release mode
RUN dotnet publish -c Release -o /app/out

# Use a runtime-only image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy the built output from the previous step
COPY --from=build /app/out ./

# Set the entry point
CMD ["dotnet", "TravelsBE.dll"]
