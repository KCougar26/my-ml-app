# 1. BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers for faster caching
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# 2. RUNTIME STAGE
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy the compiled app from the build stage
COPY --from=build /app/out .

# Copy the SQLite database into the /app folder
# This assumes shop.db is in the same folder as your Dockerfile
COPY shop.db .

# Permissions fix: SQLite requires write access to the folder 
# and the file to manage journal/temp files.
USER root
RUN chmod 777 /app
RUN chmod 666 /app/shop.db

# Standard Render Ports
EXPOSE 80
EXPOSE 8080

ENTRYPOINT ["dotnet", "Backend.dll"]