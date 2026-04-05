# Use the .NET 10 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code and publish the app
COPY . ./
RUN dotnet publish -c Release -o out

# Use the .NET 10 ASP.NET Runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .

# Render (and most cloud providers) expects traffic on port 80 or 8080
EXPOSE 80
EXPOSE 8080

COPY shop.db .

# IMPORTANT: Change 'Backend.dll' to your actual project name (e.g., WaterProject.dll)
ENTRYPOINT ["dotnet", "backend.dll"]