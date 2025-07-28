#!/bin/bash

set -e

# Configuration
ACR_NAME="${ACR_NAME:-acr45fiy7ic566ao}"
RESOURCE_GROUP="${RESOURCE_GROUP:-rg-prd}"
IMAGE_TAG="${1:-latest}"
BACKEND_URL="${BACKEND_URL:-https://apiservice.wonderfulrock-4e9ab187.eastus2.azurecontainerapps.io}"

echo "🏗️ Building and pushing frontend container..."
echo "ACR: $ACR_NAME"
echo "Image Tag: $IMAGE_TAG"
echo "Backend URL: $BACKEND_URL"

# Login to ACR
echo "🔐 Logging in to ACR..."
az acr login --name $ACR_NAME

# Get ACR login server
ACR_LOGIN_SERVER=$(az acr show --name $ACR_NAME --resource-group $RESOURCE_GROUP --query loginServer --output tsv)
echo "ACR Login Server: $ACR_LOGIN_SERVER"

# Build the container with build args
echo "🔨 Building container image..."
cd src/RMA.Frontend

docker build \
  --build-arg BACKEND_URL="$BACKEND_URL" \
  --build-arg NODE_ENV=production \
  -t $ACR_LOGIN_SERVER/rma-frontend:$IMAGE_TAG \
  -t $ACR_LOGIN_SERVER/rma-frontend:latest \
  .

# Push both tags
echo "📤 Pushing container image..."
docker push $ACR_LOGIN_SERVER/rma-frontend:$IMAGE_TAG
docker push $ACR_LOGIN_SERVER/rma-frontend:latest

echo "✅ Frontend container built and pushed successfully!"
echo "Image: $ACR_LOGIN_SERVER/rma-frontend:$IMAGE_TAG"

# Test the image locally (optional)
echo "🧪 Testing container locally..."
docker run --rm -d -p 8080:80 \
  -e BACKEND_URL="$BACKEND_URL" \
  --name test-frontend \
  $ACR_LOGIN_SERVER/rma-frontend:$IMAGE_TAG

sleep 5

# Test health endpoint
if curl -f http://localhost:8080/health; then
    echo "✅ Container health check passed"
else
    echo "❌ Container health check failed"
fi

# Test config.js
if curl -f http://localhost:8080/config.js; then
    echo "✅ Config.js accessible"
else
    echo "❌ Config.js not accessible"
fi

# Stop test container
docker stop test-frontend

echo "🎉 Build and push completed successfully!"