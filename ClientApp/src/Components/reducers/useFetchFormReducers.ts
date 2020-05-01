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

/**
 * Type determining all possible form actions
 */
export type FormActions<T> =
    | UpdateFieldsAction<T>
    | UpdateFieldAction<T>
    | ResetForm;

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
        default:
            // If you don't handle all possible values for action.type, ESLint will complain here
            neverReached(reducerAction);
    }
    return state;
};

/**
 * The reducer can't be generic, so we have to wrap it in a generator function
 */
// export const fetchFormReducerGenerator = <T extends {}>() => (
//     state: {
//         props: FormFieldProps<T>;
//         triggerSubmit: number;
//     },
//     reducerAction: FormActions<T>
// ): {
//     props: FormFieldProps<T>;
//     triggerSubmit: number;
// } => {
//     // a function which updates just the specified field state
//     const getFieldState = (field: FieldValue<T>) =>
//         // TODO the typing wouldn't pick up that these are compatible, so I explicitly typed it
//         // eslint-disable-next-line @typescript-eslint/no-object-literal-type-assertion
//         ({
//             [field.property]: {
//                 ...state.props[field.property],
//                 attributes: {
//                     ...state.props[field.property]?.attributes,
//                     value: field.value,
//                 },
//             },
//         } as FieldProps<T>);

//     switch (reducerAction.type) {
//         case 'UPDATE_FIELD':
//             return {
//                 ...state,
//                 ...getFieldState(reducerAction.field),
//             };
//         case 'UPDATE_FIELDS': {
//             // fist, map all the fields to there updated state, then reduce
//             // to a single instance of FieldProps, and finally merge with
//             // existing state.
//             const fields = reducerAction.fields
//                 .map(getFieldState)
//                 .reduce((s: FieldProps<T>, p) => ({ ...s, ...p }));

//             return {
//                 ...state,
//                 ...fields,
//             };
//         }
//         // reset all form values
//         case 'RESET_FORM':
//             return {
//                 ...state,
//                 ...mergeAttributes(state.props, () => ({ value: '' })),
//             };
//         default:
//             // If you don't handle all possible values for action.type, ESLint will complain here
//             neverReached(reducerAction);
//     }
//     return state;
// };
