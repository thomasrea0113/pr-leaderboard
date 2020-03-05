module.exports = {
    env: {
        browser: true,
        es6: true,
        'jest/globals': true,
    },
    extends: [
        'airbnb',
        'prettier',
        'prettier/babel',
        'prettier/react',
        'prettier/standard',
        'prettier/@typescript-eslint',
        'plugin:@typescript-eslint/recommended',
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
    plugins: ['jest', 'import', 'react', '@typescript-eslint', 'prettier'],
    rules: {
        'prettier/prettier': 'error',
        'react/jsx-filename-extension': [
            2,
            {
                extensions: ['.tsx'],
            },
        ],

        // the typescript tooling in vscode will handle this for us
        'no-undef': 0,
        'jsx-a11y/label-has-associated-control': [
            2,
            {
                assert: 'htmlFor',
            },
        ],

        // the babel-plugin-typescript-to-proptypes package will auto-generate propTypes for us
        'react/prop-types': 0,
        '@typescript-eslint/indent': 0,
        '@typescript-eslint/explicit-function-return-type': 0,
        '@typescript-eslint/no-parameter-properties': 0,
        'no-empty-function': [2, { allow: ['constructors'] }],
        'import/no-unresolved': [2, { commonjs: true, amd: true }],
        'import/named': 2,
        'no-underscore-dangle': [2, { allowAfterThis: true }],
        'import/prefer-default-export': 0,
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
        'import/extensions': ['.ts', '.tsx'],
    },
};
