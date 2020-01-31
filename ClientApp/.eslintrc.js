module.exports = {
    env: {
        browser: true,
        es6: true,
    },
    extends: [
        'airbnb',
        'prettier',
        'prettier/@typescript-eslint',
        'prettier/babel',
        'prettier/react',
        'prettier/standard',
        'plugin:import/errors',
        'plugin:import/warnings',
        'plugin:import/typescript',
    ],
    globals: {
        Atomics: 'readonly',
        SharedArrayBuffer: 'readonly',
    },
    parser: '@typescript-eslint/parser',
    parserOptions: {
        ecmaFeatures: {
            jsx: true,
        },
        ecmaVersion: 2018,
        sourceType: 'module',
    },
    plugins: ['import', 'react', '@typescript-eslint', 'prettier'],
    ignorePatterns: ['*.ejs', 'node_modules/'],
    rules: {
        'prettier/prettier': 'error',
        'react/jsx-filename-extension': [
            2,
            {
                extensions: ['.tsx'],
            },
        ],
        'import/no-unresolved': [2, { commonjs: true, amd: true }],
        'import/named': 2,
        'import/namespace': 2,
        'import/default': 2,
        'import/export': 2,
        'import/extensions': [
            2,
            {
                extensions: ['.js', '.json', '.jsx', '.ts', '.tsx', 'css'],
            },
        ],
        'react/static-property-placement': ['error', 'static getter'],
    },
    settings: {
        'import/extensions': ['.tsx'],
    },
};
