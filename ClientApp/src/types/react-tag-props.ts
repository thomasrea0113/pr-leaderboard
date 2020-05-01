// shorthand types for react component properties for standard html tags

export type ReactButtonProps = React.FC<
    React.DetailedHTMLProps<
        React.ButtonHTMLAttributes<HTMLButtonElement>,
        HTMLButtonElement
    >
>;

export type ReactFormProps = React.DetailedHTMLProps<
    React.FormHTMLAttributes<HTMLFormElement>,
    HTMLFormElement
>;

export type ReactInputProps = React.DetailedHTMLProps<
    React.InputHTMLAttributes<HTMLInputElement>,
    HTMLInputElement
>;

export type ReactTextareaProps = React.DetailedHTMLProps<
    React.TextareaHTMLAttributes<HTMLTextAreaElement>,
    HTMLTextAreaElement
>;

export type ReactSelectProps = React.DetailedHTMLProps<
    React.SelectHTMLAttributes<HTMLSelectElement>,
    HTMLSelectElement
>;

/**
 * props for any possible child of a form element
 */
// TODO enable support for other input types
export type ReactFormFieldProps = ReactInputProps;
// export type ReactFormFieldProps =
//     | ReactInputProps
//     | ReactTextareaProps
//     | ReactSelectProps;

/**
 * the possible types for an input value
 */
export type HTMLInputValue = string | number | string[] | undefined;

/**
 * defines a type that can be mapped to the value of a form field for a given type
 */
export type FormFieldValues<T> = Partial<Record<keyof T, HTMLInputValue>>;

/**
 * contains the react props for all the form fields of a given type
 */
export type FormFieldProps<T> = Partial<Record<keyof T, ReactFormFieldProps>>;
