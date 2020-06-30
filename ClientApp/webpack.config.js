/* eslint-disable @typescript-eslint/no-var-requires */
const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const { StatsWriterPlugin } = require('webpack-stats-plugin');

const outDir = path.resolve(__dirname, 'dist');

module.exports = () => {
    const config = {
        mode: 'development',
        entry: {
            recommendations: ['./src/pages/Recommendations.tsx'],
            home: ['./src/pages/Home.tsx'],
            site: ['./src/Site.ts'],
            'board/view': ['./src/pages/Board/View.tsx'],
            admin: ['./src/pages/Admin.tsx'],
        },
        output: {
            path: outDir,
            filename: 'js/[name].[hash].js',

            // exposing all components in a global Componentes variable
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
                    loader: ['babel-loader', 'ts-loader', 'eslint-loader'],
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
                {
                    test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
                    use: [
                        {
                            loader: 'url-loader',
                            options: {
                                name: '[name].[ext]',
                                outputPath: '/fonts/',
                            },
                        },
                    ],
                },
                {
                    test: require.resolve('react'),
                    use: [
                        {
                            loader: 'expose-loader',
                            options: 'React',
                        },
                    ],
                },
                {
                    test: require.resolve('react-dom'),
                    use: [
                        {
                            loader: 'expose-loader',
                            options: 'ReactDOM',
                        },
                    ],
                },
                {
                    test: require.resolve('jquery'),
                    use: [
                        {
                            loader: 'expose-loader',
                            options: 'jQuery',
                        },
                        {
                            loader: 'expose-loader',
                            options: '$',
                        },
                    ],
                },
            ],
        },
        plugins: [
            // instead of using devtool, we will build the source pags automatically
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map',
            }),
            new MiniCssExtractPlugin({
                filename: 'css/[name].[hash].css',
            }),
            // used by the Razor Page TagHelper. The helper reads the stats file to
            // figured out the virtual path of the bundles
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

    config.mode = process.env.NODE_ENV || 'development';

    if (config.mode === 'development') {
        config.devServer = {
            contentBase: outDir,
            compress: true,
            host: '0.0.0.0',
            port: 9000,
            hot: true,
            writeToDisk: file => path.basename(file) === 'stats.json',
            headers: {
                'Access-Control-Allow-Origin': '*',
            },
        };

        config.output.publicPath = 'http://localhost:9000';
    }

    return config;
};
