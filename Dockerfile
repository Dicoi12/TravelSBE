# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY *.csproj ./
RUN dotnet restore

# Copy the entire project and publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/publish ./

# Set the entry point for the application
ENTRYPOINT ["dotnet", "TravelSBE.dll"]
