#!/bin/sh

#!/bin/sh

# Docker entrypoint script for injecting environment variables into config.js
set -e

echo "🚀 Starting RFID Frontend Application..."

# Set default values for environment variables if not provided
export BACKEND_URL="${BACKEND_URL:-}"

echo "📝 Generating runtime configuration..."

# Create config.js with only backend URL
cat > /usr/share/nginx/html/config.js << EOF
window.ENV = {
  BACKEND_URL: "${BACKEND_URL}"
};
EOF

echo "✅ Configuration generated successfully"

# Print configuration for debugging (without sensitive tokens)
echo "🔧 Runtime Configuration:"
echo "  - Keycloak URL: $BACKEND_URL"

# Generate nginx configuration with environment variables if template exists
if [ -f "/etc/nginx/conf.d/default.conf.template" ]; then
  echo "🌐 Configuring Nginx from template..."
  envsubst '$BACKEND_URL' < /etc/nginx/conf.d/default.conf.template > /etc/nginx/conf.d/default.conf
  
  # Validate nginx configuration
  echo "🔍 Validating Nginx configuration..."
  nginx -t
  if [ $? -eq 0 ]; then
      echo "✅ Nginx configuration is valid"
  else
      echo "❌ Nginx configuration is invalid, please check the template"
      cat /etc/nginx/conf.d/default.conf
      exit 1
  fi
fi

echo "🚀 Starting Nginx..."
exec nginx -g 'daemon off;'
