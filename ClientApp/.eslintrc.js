module.exports = {
    env: {
        browser: true,
        es6: true,
        'jest/globals': true,
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
        'plugin:@typescript-eslint/recommended',
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
    plugins: ['jest', 'import', 'react', '@typescript-eslint', 'prettier'],
    rules: {
        'prettier/prettier': 'error',
        'react/jsx-filename-extension': [
            2,
            {
                extensions: ['.tsx'],
            },
        ],
        '@typescript-eslint/indent': 0,
        '@typescript-eslint/explicit-function-return-type': 0,
        '@typescript-eslint/no-parameter-properties': 0,
        'no-empty-function': [2, { allow: ['constructors'] }],
        'import/no-unresolved': [2, { commonjs: true, amd: true }],
        'import/named': 2,
        'no-underscore-dangle': [2, { allowAfterThis: true }],
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
        'react/jsx-props-no-spreading': 0,

        'jest/no-disabled-tests': 'warn',
        'jest/no-focused-tests': 'error',
        'jest/no-identical-title': 'error',
        'jest/prefer-to-have-length': 'warn',
        'jest/valid-expect': 'error',
    },
    settings: {
        'import/extensions': ['.tsx'],
    },
};
