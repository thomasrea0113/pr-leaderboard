/* eslint-disable @typescript-eslint/no-var-requires */
const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const { StatsWriterPlugin } = require('webpack-stats-plugin');

module.exports = {
    mode: 'development',
    entry: {
        recommendations: ['./src/pages/Recommendations.tsx'],
        home: ['./src/pages/Home.tsx'],
        site: ['./src/Site.ts'],
    },
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'js/[name].[hash].js',
        library: ['Components', '[name]'],
        libraryTarget: 'var',
        libraryExport: 'default',
    },
    devtool: false,
    resolve: {
        extensions: ['.js', '.jsx', '.json', '.ts', '.tsx'],
    },
    module: {
        rules: [
            {
                test: /\.ts(x|)$/,
                loader: ['babel-loader', 'ts-loader'],
                exclude: /node_modules/,
            },
            {
                test: /\.js/,
                exclude: /node_modules/,
                enforce: 'pre',
                loader: ['source-map-loader'],
            },
            {
                test: /\.s[ac]ss$/,
                exclude: /node_modules/,
                use: [
                    MiniCssExtractPlugin.loader,
                    {
                        loader: 'css-loader',
                        options: {
                            sourceMap: true,
                        },
                    },
                    'resolve-url-loader',
                    {
                        loader: 'sass-loader',
                        options: {
                            sourceMap: true,
                        },
                    },
                ],
            },
            {
                test: /\.css$/,
                include: /node_modules/,
                use: [
                    MiniCssExtractPlugin.loader, // will export css to a separate file
                    // 'style-loader', // would inject the css via javascript
                    'css-loader',
                ],
            },
        ],
    },
    plugins: [
        new webpack.SourceMapDevToolPlugin({
            filename: '[file].map',
        }),
        new MiniCssExtractPlugin({
            filename: 'css/[name].[hash].css',
        }),
        new StatsWriterPlugin({
            stats: {
                all: false,
                assets: true,
                hash: true,
                publicPath: true,
            },
        }),
    ],
};
