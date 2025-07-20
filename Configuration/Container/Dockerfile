# Use the official Microsoft development container image
FROM mcr.microsoft.com/devcontainers/dotnet:1-9.0-bookworm

# Set the working directory
WORKDIR /workspace

# Install additional tools and dependencies
RUN apt-get update && export DEBIAN_FRONTEND=noninteractive \
    && apt-get -y install --no-install-recommends \
        curl \
        wget \
        unzip \
        git \
        build-essential \
        libssl-dev \
        libffi-dev \
        python3-dev \
        python3-pip \
        jq \
        vim \
        nano \
        htop \
        postgresql-client \
        redis-tools \
        # Install Node.js LTS
        ca-certificates \
        gnupg \
        lsb-release \
    && curl -fsSL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    # Install Docker CLI
    && curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null \
    && apt-get update \
    && apt-get install -y docker-ce-cli \
    # Install GitHub CLI
    && curl -fsSL https://cli.github.com/packages/githubcli-archive-keyring.gpg | dd of=/usr/share/keyrings/githubcli-archive-keyring.gpg \
    && chmod go+r /usr/share/keyrings/githubcli-archive-keyring.gpg \
    && echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/githubcli-archive-keyring.gpg] https://cli.github.com/packages stable main" | tee /etc/apt/sources.list.d/github-cli.list > /dev/null \
    && apt-get update \
    && apt-get install -y gh \
    # Clean up
    && apt-get autoremove -y \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# Switch to vscode user
USER vscode

# Install .NET global tools
RUN dotnet tool install -g dotnet-ef \
    && dotnet tool install -g dotnet-aspnet-codegenerator \
    && dotnet tool install -g dotnet-reportgenerator-globaltool \
    && dotnet tool install -g dotnet-format \
    && dotnet tool install -g dotnet-outdated-tool \
    && dotnet tool install -g dotnet-trace \
    && dotnet tool install -g dotnet-dump \
    && dotnet tool install -g dotnet-counters \
    && dotnet tool install -g Microsoft.dotnet-httprepl \
    && dotnet tool install -g dotnet-sonarscanner \
    && dotnet tool install -g Microsoft.Web.LibraryManager.Cli \
    && dotnet tool install -g dotnet-dev-certs

# Install npm global packages
RUN npm install -g \
    playwright \
    @playwright/test \
    typescript \
    @types/node \
    eslint \
    prettier \
    @typescript-eslint/parser \
    @typescript-eslint/eslint-plugin

# Create directories for caching
RUN mkdir -p /home/vscode/.dotnet/tools \
    && mkdir -p /home/vscode/.nuget/packages \
    && mkdir -p /home/vscode/.npm

# Set environment variables
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV DOTNET_NOLOGO=1
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
ENV NUGET_PACKAGES=/home/vscode/.nuget/packages
ENV PATH="/home/vscode/.dotnet/tools:${PATH}"

# Install Playwright browsers
RUN npx playwright install --with-deps

# Set the default working directory
WORKDIR /workspace

# Expose common development ports
EXPOSE 5000 5001 18080 18081 18082 18083 18084 18085

# Keep container running
CMD ["sleep", "infinity"]