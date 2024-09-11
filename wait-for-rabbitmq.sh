#!/bin/sh
set -e

host="$1"
shift
cmd="$@"

echo "Waiting for RabbitMQ at $host:5672..."


until curl -s -o /dev/null "http://$host:15672"; do
  >&2 echo "RabbitMQ is unavailable - sleeping"
  sleep 3
done

>&2 echo "RabbitMQ is up - executing command"
exec $cmd