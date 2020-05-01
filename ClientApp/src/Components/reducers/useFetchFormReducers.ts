import { neverReached } from '../../utilities/neverReached';
import { FormFieldProps, HTMLInputValue } from '../../types/react-tag-props';

/**
 * denotes the value for a single field of T
 */
export interface FieldValue<T> {
    property: keyof T;
    value: HTMLInputValue;
}

/**
 * actions to update a single field
 */
export interface UpdateFieldAction<T> {
    readonly type: 'UPDATE_FIELD';
    readonly field: FieldValue<T>;
}

/**
 * Action to update many fields
 */
export interface UpdateFieldsAction<T> {
    readonly type: 'UPDATE_FIELDS';
    readonly fields: FieldValue<T>[];
}

/**
 * Action to reset all form values
 */
export interface ResetForm {
    readonly type: 'RESET_FORM';
}

export interface SubmitForm {
    readonly type: 'SUBMIT_FORM';
}

/**
 * Type determining all possible form actions
 */
export type FormActions<T> =
    | UpdateFieldsAction<T>
    | UpdateFieldAction<T>
    | ResetForm
    | SubmitForm;

export interface FetchFormReducerState<T> {
    props: FormFieldProps<T>;
    triggerSubmit: number;
}

/**
 * merges the value specified in field into the reducer's state.
 * Can be used to reduce an array of FieldValue<T> into an instance
 * of FetchFormReducerState<T>
 * @param field the reducer action field
 */
export const mergeFieldValue = <T extends {}>(
    state: FetchFormReducerState<T>,
    field: FieldValue<T>
): FetchFormReducerState<T> => ({
    ...state,
    props: {
        ...state.props,
        [field.property]: {
            ...state.props[field.property],
            value: field.value,
        },
    },
});

/**
 * a function to reset the form fields by mapping the keys to a FieldValue,
 * then reducing in the same manner as setting many fields
 */
export const resetAllFieldValues = <T extends {}>(
    state: FetchFormReducerState<T>
): FetchFormReducerState<T> =>
    Object.keys(state.props)
        .map<FieldValue<T>>(k => ({ property: k as keyof T, value: '' }))
        .reduce(mergeFieldValue, state);

// returns a non-generic form reducer given the type T
export const fetchFormReducerGenerator = <T extends {}>() => (
    state: FetchFormReducerState<T>,
    reducerAction: FormActions<T>
): {
    props: FormFieldProps<T>;
    triggerSubmit: number;
} => {
    switch (reducerAction.type) {
        case 'UPDATE_FIELD':
            return mergeFieldValue(state, reducerAction.field);
        case 'UPDATE_FIELDS': {
            return reducerAction.fields.reduce(mergeFieldValue, state);
        }
        case 'RESET_FORM':
            return resetAllFieldValues(state);
        case 'SUBMIT_FORM':
            return { ...state, triggerSubmit: state.triggerSubmit + 1 };
        default:
            // If you don't handle all possible values for action.type, ESLint will complain here
            neverReached(reducerAction);
    }
    return state;
};
