const webpack = require('webpack');
const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

module.exports = function override(config, env) {
    config.entry = {
        home: './src/pages/home.js'
    };

    config.externals = {
        react: 'React',
        'react-dom': 'ReactDOM'
    }

    // disable code splitting for easier integration into the core app
    config.optimization.splitChunks = {
        cacheGroups: {
           default: false
        }
    }

    config.output.libraryTarget = 'var';
    config.output.libraryExport = '';
    config.output.library = 'Components'

    config.mode = 'development';

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