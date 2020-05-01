import React, {
    useReducer,
    ChangeEventHandler,
    RefObject,
    useEffect,
} from 'react';
import flow from 'lodash/fp/flow';
import toPairs from 'lodash/fp/toPairs';
import map from 'lodash/fp/map';
import fromPairs from 'lodash/fp/fromPairs';

import { useLoading, UseLoading } from './useLoading';
import { HttpMethodsEnum } from '../types/types';
import { LoadingIcon } from '../Components/StyleComponents';
import {
    ReactFormProps,
    ReactButtonProps,
    FormFieldProps,
    FormFieldValues,
    ReactFormFieldProps,
    HTMLInputValue,
} from '../types/react-tag-props';
import {
    FormActions,
    fetchFormReducerGenerator,
    FetchFormReducerState,
    FieldValue,
    mergeFieldValue,
} from '../Components/reducers/useFetchFormReducers';
import { ValidationInstance } from './useValidation';
import { isValidationErrorResponseData } from '../types/ValidationErrorResponse';

/**
 * The FetchForm instance. T is the type of the form, R is the type that is
 * returned from the server
 */
export interface FetchForm<T, R = T> {
    formProps: ReactFormProps;
    formDispatch: React.Dispatch<FormActions<T>>;
    fieldAttributes: FormFieldProps<T>;

    /**
     * submits the form, and returns a promise that indicates whether or not
     * the form was submitted
     */
    submitForm: () => Promise<boolean>;
    SubmitButton: ReactButtonProps;
    loadingProps: UseLoading<R>;
}

/**
 * props to be passed to useFetchForm. T is the type of the posted
 * object, R is the response object
 */
export interface UseFetchFormProps<T, R = T> {
    actionUrl: string;
    actionMethod: HttpMethodsEnum;

    fieldAttributes?: FormFieldProps<T>;
    initialValues?: FormFieldValues<T>;

    // if provided, automatic validation will be performed
    formRef?: RefObject<HTMLFormElement>;

    onValidSubmit?: (value: void) => void | PromiseLike<void>;

    // this is the signature for catching a promise
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onSubmitError?: (reason: any) => void;

    /**
     * gaurd the data returned after submit
     */
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    guard?: (responseData: any) => responseData is R;

    /**
     * if provided, will enforce form validation
     */
    validationInstance?: ValidationInstance<T>;
}

/**
 * extracts just the form values
 */
const getFormValues = <T extends {}>(
    props: FormFieldProps<T>
): FormFieldValues<T> =>
    Object.keys(props).reduce<FormFieldValues<T>>((values, p) => {
        // we know p is a key of T, based on the type of props
        const pKey = p as keyof T;
        const propAtributes: ReactFormFieldProps | undefined = props[pKey];
        return {
            ...values,
            [pKey]: propAtributes?.value,
        };
    }, {});

/**
 * A hook that allows for easily converting any form to an asyncronous fetch operation
 * @param props props needed for fetch overloads
 */
export const useFetchForm = <T extends {}, R = T>({
    actionUrl,
    actionMethod,

    fieldAttributes,
    initialValues,
    onValidSubmit,
    onSubmitError: onError,
    validationInstance,
    guard,
}: UseFetchFormProps<T, R>): FetchForm<T, R> => {
    const { validator, showErrors } = { ...validationInstance };

    const initialReducerState: FetchFormReducerState<T> = {
        triggerSubmit: 0,
        props: fieldAttributes ?? {},
    };

    // if initial values are provided, convert them to a FieldValue array
    const initialFieldValues =
        initialValues != null
            ? Object.keys(initialValues).map<FieldValue<T>>(k => {
                  const fieldKey = k as keyof T;
                  const initialValue = initialValues[fieldKey];
                  return {
                      property: fieldKey,
                      value: initialValue,
                  };
              })
            : null;

    // initialize reducer
    const [{ props: formFieldProps, triggerSubmit }, formDispatch] = useReducer(
        fetchFormReducerGenerator<T>(),
        // if initial values are provided, we need to merge it into the
        // attributes before initializing the state
        initialFieldValues != null
            ? initialFieldValues.reduce(mergeFieldValue, initialReducerState)
            : initialReducerState
    );

    // a convenient function to generate an onChange event hanlder for
    // a given property of T
    const onChangeGenerator = (property: keyof T) => {
        // we don't care what type of event we're handling, as long as
        // the element has an input value attribute
        const evt: ChangeEventHandler<{ value: HTMLInputValue }> = ({
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

    const loadingProps = useLoading<R>({ guard });
    const { isLoading, loadAsync, response } = loadingProps;

    // merge in the onChange generator, and disabled property
    type ReactFormFieldPropPair = [keyof T, ReactFormFieldProps];
    const fieldProps: FormFieldProps<T> = flow(
        toPairs,
        map<ReactFormFieldPropPair, ReactFormFieldPropPair>(([k, v]) => [
            k,
            { ...v, onChange: onChangeGenerator(k), disabled: isLoading },
        ]),
        fromPairs
    )(formFieldProps);

    const SubmitButton: ReactButtonProps = props => {
        const { children } = props;
        return (
            <button {...props} type="submit" disabled={isLoading}>
                <LoadingIcon isLoading={isLoading} />
                {children}
            </button>
        );
    };

    const submitForm = (): Promise<boolean> => {
        if (validator == null || validator.form()) {
            const formValues = getFormValues(formFieldProps);
            return loadAsync({
                actionUrl,
                actionMethod,
                body: JSON.stringify(formValues),
                additionalHeaders: {
                    'Content-Type': 'application/json',
                },
            })
                .then(onValidSubmit)
                .catch(onError)
                .then(() => true); // this is here to prevent the returned promised from having any information
        }
        return new Promise(() => false);
    };

    useEffect(() => {
        if (triggerSubmit > 0) submitForm();
    }, [triggerSubmit]);

    if (
        showErrors != null &&
        response?.data != null &&
        isValidationErrorResponseData<T>(response.data)
    )
        showErrors(response.data.errors);

    return {
        formProps: {
            onSubmit: async e => {
                // make sure the form submits using our handler
                e.preventDefault();
                submitForm();
            },
        },
        formDispatch,
        submitForm,
        loadingProps,
        SubmitButton,
        fieldAttributes: fieldProps,
    };
};
