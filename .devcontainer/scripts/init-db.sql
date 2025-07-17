-- Initialize MCP Hub development database
-- This script runs when the PostgreSQL container starts

-- Create extensions needed for the application
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
CREATE EXTENSION IF NOT EXISTS "btree_gin";
CREATE EXTENSION IF NOT EXISTS "btree_gist";

-- Create a schema for the application
CREATE SCHEMA IF NOT EXISTS mcphub;

-- Grant permissions
GRANT ALL PRIVILEGES ON SCHEMA mcphub TO mcphub_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA mcphub TO mcphub_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA mcphub TO mcphub_user;

-- Set default search path
ALTER USER mcphub_user SET search_path TO mcphub, public;

-- Create some basic configuration
INSERT INTO pg_settings (name, setting) VALUES ('shared_preload_libraries', 'pg_stat_statements')
ON CONFLICT (name) DO NOTHING;

-- Log initialization
SELECT 'MCP Hub development database initialized successfully' AS status;