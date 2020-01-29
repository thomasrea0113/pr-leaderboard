const webpack = require('webpack')
const path = require('path');

module.exports = function override(config, env) {
    config.entry = {
        home: './src/pages/home.js',
        // site: './src/site.js'
    };

    return config;
}