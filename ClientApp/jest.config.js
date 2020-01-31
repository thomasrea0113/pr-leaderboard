module.exports = {
    preset: 'ts-jest',
    testEnvironment: 'node',
    testRegex: '(/src/.*\\.test)\\.tsx?$',
    moduleNameMapper: {
        '\\.(css|s[ac]ss)$': 'identity-obj-proxy',
    },
};
