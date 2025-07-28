#!/bin/sh

# Docker entrypoint script for injecting environment variables into config.js
set -e

echo "🚀 Starting RMA Frontend Application..."

# Set default values for environment variables if not provided
export BACKEND_URL="${BACKEND_URL:-http://localhost:5450}"
export NODE_ENV="${NODE_ENV:-production}"

echo "📝 Generating runtime configuration..."

# Create config.js with backend URL (runtime injection)
cat > /usr/share/nginx/html/config.js << EOF
window.ENV = {
  BACKEND_URL: "${BACKEND_URL}",
  NODE_ENV: "${NODE_ENV}",
  VERSION: "$(date +%Y%m%d-%H%M%S)"
};
EOF

echo "✅ Configuration generated successfully"

# Print configuration for debugging (mask sensitive data)
echo "🔧 Runtime Configuration:"
echo "  - Backend URL: $BACKEND_URL"
echo "  - Environment: $NODE_ENV"

# Generate nginx configuration from template if it exists
if [ -f "/etc/nginx/conf.d/default.conf.template" ]; then
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
else
    echo "ℹ️ Using default nginx configuration"
fi

# Ensure proper file permissions
chown -R nginx:nginx /usr/share/nginx/html
chmod -R 755 /usr/share/nginx/html

echo "🚀 Starting Nginx..."

# Create a simple signal handler to prevent unexpected shutdowns
trap 'echo "Received shutdown signal, stopping gracefully..."; nginx -s quit; exit 0' TERM INT QUIT

# Start nginx in foreground
exec nginx -g 'daemon off;'