import React, { useReducer, ChangeEventHandler } from 'react';
import flow from 'lodash/fp/flow';
import toPairs from 'lodash/fp/toPairs';
import fromPairs from 'lodash/fp/fromPairs';
import map from 'lodash/fp/map';
import { parseCookie } from '../../Site';
import { neverReached } from '../../utilities/neverReached';
import { FieldProps, FieldPropInfo } from './Validation';

export type ReactFormProps = React.DetailedHTMLProps<
    React.FormHTMLAttributes<HTMLFormElement>,
    HTMLFormElement
>;

export interface Action {
    readonly type: string;
}

export interface SubmitFormAction extends Action {
    readonly type: 'SUBMIT_FORM';
}

export interface UpdateFieldAction<T extends {}> extends Action {
    readonly type: 'UPDATE_FIELD';
    readonly property: keyof T;
    readonly value: string;
}

export type FormActions<T> = UpdateFieldAction<T> | SubmitFormAction;

export interface FetchForm<T> {
    formProps: ReactFormProps;
    formDispatch: React.Dispatch<FormActions<T>>;
    fieldAttributes: FieldProps<T>;
}

export interface UseFetchFormProps<T> {
    fieldAttributes: FieldProps<T>;
}

export const fetchFormReducerGenerator = <T extends {}>() => (
    state: FieldProps<T>,
    reducerAction: FormActions<T>
): FieldProps<T> => {
    switch (reducerAction.type) {
        case 'UPDATE_FIELD': {
            return {
                ...state,
                [reducerAction.property]: {
                    ...state[reducerAction.property],
                    attributes: {
                        ...state[reducerAction.property].attributes,
                        value: reducerAction.value,
                    },
                },
            };
        }
        case 'SUBMIT_FORM':
            break;
        default:
            // If you don't handle all possible values for action.type, ESLint will complain here
            neverReached(reducerAction);
    }
    return state;
};

/**
 * A hook that allows for easily converting any form to an asyncronous fetch operation
 * @param props props needed for fetch overloads
 */
export const useFetchForm = <T extends {}>({
    fieldAttributes,
}: UseFetchFormProps<T>): FetchForm<T> => {
    const csrf = parseCookie().requestVerificationToken;
    if (csrf == null) throw new Error('csrf token not found');

    const [formState, formDispatch] = useReducer(
        fetchFormReducerGenerator<T>(),
        fieldAttributes
    );

    // a convenient function to generate an onChange event hanlder for
    // a given property of T
    const onChangeGenerator = (property: keyof T) => {
        const evt: ChangeEventHandler<HTMLInputElement> = ({
            currentTarget: { value },
        }) =>
            formDispatch({
                type: 'UPDATE_FIELD',
                property,
                value,
            });
        return evt;
    };

    type PairType = [keyof T, FieldPropInfo];

    // merge the passed in props with onChange handler
    // first we convert the object to a key-value array (typed above)
    // then map that array to a similar aray but with the onChange
    // handler appended, and map back to an object
    const fieldProps = flow(
        toPairs,
        map<PairType, PairType>(kv => [
            kv[0],
            {
                ...kv[1],
                attributes: {
                    ...kv[1].attributes,
                    onChange: onChangeGenerator(kv[0]),
                },
            },
        ]),
        fromPairs
    )(formState) as FieldProps<T>;

    return {
        formProps: {
            onSubmit: async e => {
                e.preventDefault();
                const target = e.target as HTMLFormElement;
                const { action, method } = target;
                await fetch(action ?? '/', {
                    method: method ?? 'get',
                    headers: {
                        RequestVerificationToken: csrf,
                        'Content-Type': 'application/json',
                    },
                    body: new FormData(target),
                });
            },
        },
        formDispatch,
        fieldAttributes: fieldProps,
    };
};
