#!/bin/sh
set -e

echo "Starting initialization script..."

# Default values (optional)
#: "${API_URL:=http://localhost:3000}"
#: "${FEATURE_X:=false}"


sed -i "s|{GATEWAY_HOST}|$ENV_GATEWAY_HOST|g" /usr/share/nginx/html/assets/config.json
sed -i "s|{GATEWAY_PORT}|$ENV_GATEWAY_PORT|g" /usr/share/nginx/html/assets/config.json


cat /usr/share/nginx/html/assets/config.json

# Start nginx
exec nginx -g "daemon off;"
