# MCP Hub Development Container
# Includes .NET 9 SDK, PostgreSQL client, Redis tools, and development utilities

FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set environment variables
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_ENVIRONMENT=Development

# Set timezone to match host system (will be overridden by TZ environment variable if set)
ENV TZ=UTC

# Install system dependencies
RUN apt-get update && apt-get install -y \
    wget \
    curl \
    ca-certificates \
    gnupg \
    lsb-release \
    unzip \
    git \
    postgresql-client \
    redis-tools \
    jq \
    vim \
    nano \
    && rm -rf /var/lib/apt/lists/*

# Install additional .NET tools
RUN dotnet tool install --global dotnet-ef && \
    dotnet tool install --global dotnet-aspnet-codegenerator && \
    dotnet tool install --global dotnet-dump && \
    dotnet tool install --global dotnet-trace && \
    dotnet tool install --global dotnet-counters

# Add .NET tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Verify installed tools
RUN dotnet --version && \
    dotnet ef --version && \
    psql --version && \
    redis-cli --version && \
    git --version

# Create working directory
WORKDIR /workspace

# Create .NET configuration directories
RUN mkdir -p /root/.dotnet && \
    mkdir -p /root/.nuget/packages && \
    chmod 755 /root/.dotnet /root/.nuget

# Configure NuGet to use faster package restore
RUN dotnet nuget locals all --clear

# Set proper .NET configuration
ENV DOTNET_ROOT=/usr/share/dotnet
ENV DOTNET_CLI_HOME=/root
ENV NUGET_PACKAGES=/root/.nuget/packages

# Create a custom entrypoint script for proper .NET environment
RUN echo '#!/bin/bash\n\
set -e\n\
\n\
# Ensure .NET directories exist\n\
mkdir -p /root/.dotnet /root/.nuget/packages\n\
\n\
# Set proper permissions\n\
chmod 755 /root/.dotnet /root/.nuget\n\
\n\
# Execute the command\n\
exec "$@"\n\
' > /usr/local/bin/dotnet-entrypoint.sh && \
    chmod +x /usr/local/bin/dotnet-entrypoint.sh

# Set the custom entrypoint
ENTRYPOINT ["/usr/local/bin/dotnet-entrypoint.sh"]

# Default command
CMD ["bash"]

# Add labels for identification
LABEL name="mcphub-dotnet-dev"
LABEL version="1.0"
LABEL description="MCP Hub .NET development container with .NET 9 SDK, PostgreSQL client, Redis tools, and development utilities"
LABEL maintainer="MCP Hub Development Team"