var webpack = require('webpack');
const fs = require('fs')
const packageJson = fs.readFileSync('./package.json')
const version = JSON.parse(packageJson).version || 0
const port = JSON.parse(packageJson).port || 8080

module.exports = {
    configureWebpack: {
        output: {
            hashFunction: "sha256"
        },
        devtool: 'source-map',
        plugins: [
            new webpack.DefinePlugin({
                'process.env': {
                    PACKAGE_VERSION: '"' + version + '"',
                    PORT: '"' + port + '"'
                }
            })
        ],
        optimization: {
            runtimeChunk: 'single',
            splitChunks: {
                chunks: 'all',
                maxInitialRequests: Infinity,
                automaticNameDelimiter: '_',
                cacheGroups: {
                    vendor: {
                        test: /[\\/]node_modules[\\/]/,
                        reuseExistingChunk: true,
                        name(module) {
                            // get the name. E.g. node_modules/packageName/not/this/part.js
                            // or node_modules/packageName
                            const packageNameMatch = module.context && module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/);
                            const packageName = packageNameMatch ? packageNameMatch[1] : 'vendor';

                            // npm package names are URL-safe, but some servers don't like @ symbols
                            return `npm.${packageName.replace('@', '')}`;
                        },
                    },
                },
            },
        },
    }
}