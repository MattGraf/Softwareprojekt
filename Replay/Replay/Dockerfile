# Use a lean container to run the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base

# create a directory for the app
WORKDIR /app
# expose the ports used by the application
EXPOSE 5000
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Use a container that contains the complete sdk to build the container
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

WORKDIR /src

COPY ["Replay.csproj", "./"]
RUN dotnet restore "Replay.csproj"

COPY . .
RUN dotnet publish "Replay.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
RUN mkdir Persistence
COPY --from=build /app/publish .
LABEL com.centurylinklabs.watchtower.enable="True"
USER root
RUN chown -R appuser /app
USER appuser
ENTRYPOINT ["dotnet", "Replay.dll"]
