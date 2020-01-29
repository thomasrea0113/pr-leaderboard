const webpack = require('webpack')
const path = require('path');

module.exports = function override(config, env) {
    config.entry = {
        home: './src/pages/Home.js',
    };

    config.plugins.push(...[
      new webpack.DefinePlugin({
        'process.env.NODE_ENV': JSON.stringify('development')
      })
    ]);

    return config;
}