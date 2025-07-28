#!/bin/sh

# Docker entrypoint script for injecting environment variables into config.js
set -e

echo "🚀 Starting RFID Frontend Application..."

# Set default values for environment variables if not provided
export BACKEND_URL="${BACKEND_URL:-}"

echo "📝 Generating runtime configuration..."

# Create config.js with backend URL
cat > /usr/share/nginx/html/config.js << EOF
window.ENV = {
  BACKEND_URL: "${BACKEND_URL}"
};
EOF

echo "✅ Configuration generated successfully"

# Print configuration for debugging
echo "🔧 Runtime Configuration:"
echo "  - Backend URL: $BACKEND_URL"

# Generate nginx configuration from template
echo "🌐 Configuring Nginx from template..."
envsubst '${BACKEND_URL}' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf

# Validate nginx configuration
echo "🔍 Validating Nginx configuration..."
if nginx -t; then
    echo "✅ Nginx configuration is valid"
else
    echo "❌ Nginx configuration is invalid"
    echo "Generated configuration:"
    cat /etc/nginx/conf.d/default.conf
    exit 1
fi

# Ensure proper signal handling
echo "🚀 Starting Nginx..."

# Create a simple signal handler to prevent unexpected shutdowns
trap 'echo "Received shutdown signal, stopping gracefully..."; nginx -s quit; exit 0' TERM INT QUIT

# Start nginx in foreground
exec nginx -g 'daemon off;'