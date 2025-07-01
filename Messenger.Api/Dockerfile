# STEP 1 - Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY Messenger.sln .
COPY Messenger.API/Messenger.API.csproj ./Messenger.API/

# Restore
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build and publish
WORKDIR /app/Messenger.API
RUN dotnet publish -c Release -o out

# STEP 2 - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/Messenger.API/out .

ENTRYPOINT ["dotnet", "Messenger.API.dll"]
