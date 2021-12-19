const redis = require('redis');
const config = require('../config/config');

// connect to redis
const redis_client = redis.createClient({
    url: config.REDIS_URL
  });

redis_client.connect();

module.exports = redis_client;