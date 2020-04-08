import React, { useReducer, ChangeEventHandler } from 'react';
import { parseCookie } from '../Site';
import { neverReached } from '../utilities/neverReached';
import {
    FieldProps,
    FormValues,
    fieldValues,
    mergeAttributes,
} from '../Components/forms/Validation';

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

export interface FieldValue<T> {
    property: keyof T;
    value: string;
}

export interface UpdateFieldAction<T> extends Action {
    readonly type: 'UPDATE_FIELD';
    readonly field: FieldValue<T>;
}

export interface UpdateFieldsAction<T> extends Action {
    readonly type: 'UPDATE_FIELDS';
    readonly fields: FieldValue<T>[];
}

export type FormActions<T> =
    | UpdateFieldsAction<T>
    | UpdateFieldAction<T>
    | SubmitFormAction;

export interface FetchForm<T> {
    formProps: ReactFormProps;
    formDispatch: React.Dispatch<FormActions<T>>;
    fieldAttributes: FieldProps<T>;
}

export interface UseFetchFormProps<T> {
    fieldAttributes: FieldProps<T>;
    initialValues?: FormValues<T>;
}

export const fetchFormReducerGenerator = <T extends {}>() => (
    state: FieldProps<T>,
    reducerAction: FormActions<T>
): FieldProps<T> => {
    // a function which updates just the specified field state
    const getFieldState = (field: FieldValue<T>) =>
        // TODO the typing wouldn't pick up that these are compatible, so I explicitly typed it
        // eslint-disable-next-line @typescript-eslint/no-object-literal-type-assertion
        ({
            [field.property]: {
                ...state[field.property],
                attributes: {
                    ...state[field.property].attributes,
                    value: field.value,
                },
            },
        } as FieldProps<T>);

    switch (reducerAction.type) {
        case 'UPDATE_FIELD':
            return {
                ...state,
                ...getFieldState(reducerAction.field),
            };
        case 'UPDATE_FIELDS': {
            // fist, map all the fields to there updated state, then reduce
            // to a single instance of FieldProps, and finally merge with
            // existing state.
            const fields = reducerAction.fields
                .map(getFieldState)
                .reduce((s: FieldProps<T>, p) => ({ ...s, ...p }));
            return {
                ...state,
                ...fields,
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
    initialValues,
}: UseFetchFormProps<T>): FetchForm<T> => {
    const csrf = parseCookie().csrfToken;
    if (csrf == null) throw new Error('csrf token not found');

    const initialAttributes =
        initialValues != null
            ? mergeAttributes(fieldAttributes, ([k]) => {
                  // only set the value if it was provided in initial
                  if (initialValues[k] != null)
                      return {
                          value: initialValues[k],
                      };
                  return {};
              })
            : fieldAttributes;

    const [formState, formDispatch] = useReducer(
        fetchFormReducerGenerator<T>(),
        // if initial values are provided, we need to merge it into the
        // attributes before initializing the state
        initialAttributes
    );

    // a convenient function to generate an onChange event hanlder for
    // a given property of T
    const onChangeGenerator = (property: keyof T) => {
        const evt: ChangeEventHandler<HTMLInputElement> = ({
            currentTarget: { value },
        }) =>
            formDispatch({
                type: 'UPDATE_FIELD',
                field: {
                    property,
                    value,
                },
            });
        return evt;
    };

    // automatically handle the onChange event with ease
    const fieldProps = mergeAttributes(formState, ([k]) => ({
        onChange: onChangeGenerator(k),
    }));

    return {
        formProps: {
            onSubmit: async e => {
                e.preventDefault();
                const target = e.target as HTMLFormElement;
                const { action, method } = target;
                const formValues = fieldValues(formState);
                await fetch(action ?? '/', {
                    method: method ?? 'get',
                    headers: {
                        'X-CSRF-TOKEN': csrf,
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(formValues),
                });
            },
        },
        formDispatch,
        fieldAttributes: fieldProps,
    };
};