# Streamline Development Container
# Includes all required tools: Maven 3.9.9, Java 17, Node.js 22.14.0, npm 10.9.2, pnpm 10.12.4

FROM node:22.14.0

# Set environment variables
ENV JAVA_HOME=/opt/java/openjdk
ENV MAVEN_HOME=/opt/maven
ENV PATH="${JAVA_HOME}/bin:${MAVEN_HOME}/bin:${PATH}"

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
    dos2unix \
    && rm -rf /var/lib/apt/lists/*

# Install OpenJDK 17
RUN mkdir -p /opt/java/openjdk && \
    cd /opt/java/openjdk && \
    wget -O openjdk.tar.gz https://github.com/adoptium/temurin17-binaries/releases/download/jdk-17.0.13%2B11/OpenJDK17U-jdk_x64_linux_hotspot_17.0.13_11.tar.gz && \
    tar -xzf openjdk.tar.gz --strip-components=1 && \
    rm openjdk.tar.gz

# Install Maven 3.9.9
RUN mkdir -p /opt/maven && \
    cd /opt/maven && \
    wget -O maven.tar.gz https://archive.apache.org/dist/maven/maven-3/3.9.9/binaries/apache-maven-3.9.9-bin.tar.gz && \
    tar -xzf maven.tar.gz --strip-components=1 && \
    rm maven.tar.gz

# Install pnpm 10.12.4
RUN npm install -g pnpm@10.12.4

# Verify Node.js and npm versions (should match base image)
RUN node --version && npm --version

# Verify installed tools
RUN java --version && \
    mvn --version && \
    pnpm --version && \
    dos2unix --version && \
    git --version

# Create working directory
WORKDIR /workspace

# Create Maven settings directory and fix permissions
RUN mkdir -p /root/.m2 && \
    chmod 755 /root/.m2

# Set proper Maven configuration to avoid entrypoint issues
ENV MAVEN_OPTS="-Duser.home=/root -Dfile.encoding=UTF-8"
ENV MAVEN_CONFIG=""

# Create a custom entrypoint script to handle Maven properly
RUN echo '#!/bin/bash\n\
set -e\n\
\n\
# Ensure .m2 directory exists\n\
mkdir -p /root/.m2\n\
\n\
# Execute the command\n\
exec "$@"\n\
' > /usr/local/bin/custom-entrypoint.sh && \
    chmod +x /usr/local/bin/custom-entrypoint.sh

# Set the custom entrypoint
ENTRYPOINT ["/usr/local/bin/custom-entrypoint.sh"]

# Default command
CMD ["bash"]

# Add labels for identification
LABEL name="streamline-dev"
LABEL version="3.0"
LABEL description="Streamline development container with Maven, Java, Node.js, npm, pnpm, dos2unix, and git"
LABEL maintainer="Ross Video Streamline Team"