# Build stage: For building the React frontend
FROM node:20-alpine AS build
WORKDIR /app

# Set build arguments for environment variables
ARG BACKEND_URL

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

# Copy source files
COPY . .

# Create .env file with build arguments
RUN echo "BACKEND_URL=${BACKEND_URL}" > .env

# Build the application
RUN npm run build

# Create a template file for runtime configuration
RUN echo 'window.ENV = {' > /app/dist/config.template.js && \
    echo '  BACKEND_URL: "${BACKEND_URL}",' >> /app/dist/config.template.js && \
    echo '};' >> /app/dist/config.template.js

# Production stage: Using Nginx to serve the application
FROM nginx:alpine

# Install gettext for envsubst
RUN apk add --no-cache gettext

# Remove default nginx configuration
RUN rm -rf /etc/nginx/conf.d/default.conf

# Copy built files to Nginx root directory
COPY --from=build /app/dist /usr/share/nginx/html
# COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy config template
COPY --from=build /app/dist/config.template.js /usr/share/nginx/html/config.template.js

# Set default environment variables for runtime
ENV BACKEND_URL=""

# Copy entrypoint script and ensure it has correct format
COPY docker-entrypoint.sh /docker-entrypoint.sh

RUN chmod +x /docker-entrypoint.sh && \
    # Fix potential line ending issues
    sed -i 's/\r$//' /docker-entrypoint.sh

# Expose port
EXPOSE 80

# Use the entrypoint script with shell
ENTRYPOINT ["/bin/sh", "/docker-entrypoint.sh"]
