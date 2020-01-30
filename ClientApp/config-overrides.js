const webpack = require('webpack');
const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = function override(config, env) {
    config.entry = {
        home: './src/pages/home.js'
    };

    // config.output.library = 'App'
    // config.output.libraryTarget = 'assign'

    config.externals = {
        react: 'React',
        'react-dom': 'ReactDOM'
    }

    config.plugins.push(...[
        new HtmlWebpackPlugin({
            title: 'Home',
            inject: false,
            template: '!!ejs-loader!src/index.ejs',
            filename: 'templates/home.html',
            chunks: ['home']
        })
    ]);

    return config;
}