import React, {
    useReducer,
    ChangeEventHandler,
    RefObject,
    useMemo,
} from 'react';
import flow from 'lodash/fp/flow';
import mapKeys from 'lodash/fp/mapKeys';
import upperFirst from 'lodash/fp/upperFirst';
import { neverReached } from '../utilities/neverReached';
import {
    FieldProps,
    FormValues,
    fieldValues,
    mergeAttributes,
} from '../Components/forms/Validation';
import { useLoading, TypedResponse } from './useLoading';
import { HttpMethodsEnum } from '../types/types';
import { LoadingIcon } from '../Components/StyleComponents';

export type ReactFormProps = React.DetailedHTMLProps<
    React.FormHTMLAttributes<HTMLFormElement>,
    HTMLFormElement
>;

export interface Action {
    readonly type: string;
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

export interface ResetForm extends Action {
    readonly type: 'RESET_FORM';
}

export type FormActions<T> =
    | UpdateFieldsAction<T>
    | UpdateFieldAction<T>
    | ResetForm;

export type Button = React.FC<
    React.DetailedHTMLProps<
        React.ButtonHTMLAttributes<HTMLButtonElement>,
        HTMLButtonElement
    >
>;

export interface FetchForm<T, R = unknown> {
    formProps: ReactFormProps;
    formDispatch: React.Dispatch<FormActions<T>>;
    fieldAttributes: FieldProps<T>;
    isSubmitting: boolean;
    submitForm: () => boolean;
    SubmitButton: Button;
    response?: TypedResponse<R>;
}

export interface UseFetchFormProps<T> {
    actionUrl: string;
    actionMethod: HttpMethodsEnum;

    fieldAttributes?: FieldProps<T>;
    initialValues?: FormValues<T>;

    // if provided, automatic validation will be performed
    formRef?: RefObject<HTMLFormElement>;

    onValidSubmit?: (value: void) => void | PromiseLike<void>;

    // this is the signature for catching a promise
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onSubmitError?: (reason: any) => any;
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
                    ...state[field.property]?.attributes,
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
        // reset all form values
        case 'RESET_FORM':
            return mergeAttributes(state, () => ({ value: '' }));
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
export const useFetchForm = <T extends {}, R = unknown>({
    actionUrl,
    actionMethod,

    fieldAttributes,
    initialValues,
    formRef,
    onValidSubmit,
    onSubmitError: onError,
}: UseFetchFormProps<T>): FetchForm<T, R> => {
    const initialAttributes =
        initialValues != null && fieldAttributes != null
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
        initialAttributes ?? {}
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

    const { isLoading, response, loadAsync } = useLoading<R>();

    const fieldProps = mergeAttributes(formState, ([k]) => ({
        onChange: onChangeGenerator(k),
        disabled: isLoading,
    }));

    const validator = useMemo(() => {
        if (formRef?.current != null) {
            $.validator.unobtrusive.parse(formRef.current);
            return $(formRef.current).validate();
        }
        return null;
    }, [formRef?.current]);

    if (
        formRef?.current != null &&
        response != null &&
        response.errorData?.errors != null
    )
        validator?.showErrors(
            flow(
                // mapValues(join(', ')), // convert array of errors to single string
                mapKeys(upperFirst) // the form expects TitleCase
            )(response.errorData.errors)
        );

    const SubmitButton: Button = props => {
        const { children } = props;
        return (
            <button {...props} type="submit" disabled={isLoading}>
                <LoadingIcon isLoading={isLoading} />
                {children}
            </button>
        );
    };

    const submitForm = () => {
        // if formRef is null, then validation isn't enabled. Otherwise, we take
        // the result of the validation
        const canSubmit = formRef == null || validator?.form() === true;

        if (canSubmit) {
            const formValues = fieldValues(formState);
            loadAsync({
                actionUrl,
                actionMethod,
                body: JSON.stringify(formValues),
                additionalHeaders: {
                    'Content-Type': 'application/json',
                },
            })
                .then(onValidSubmit)
                .catch(onError);
            return true;
        }
        return false;
    };

    return {
        formProps: {
            onSubmit: async e => {
                // if formRef is null, then validation isn't enabled. Otherwise, we take
                // the result of the validation
                e.preventDefault();
                submitForm();
            },
        },
        formDispatch,
        submitForm,
        SubmitButton,
        fieldAttributes: fieldProps,
        isSubmitting: isLoading,
        response,
    };
};
