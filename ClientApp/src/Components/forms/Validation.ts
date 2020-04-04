/**
 * Has all the same properties as the standard input element, except the
 * name attribute must be match one of the properties of the type parameter.
 * this is useful for type checking forms that are going to be posted
 */

export type HtmlInputProps = React.DetailedHTMLProps<
    React.InputHTMLAttributes<HTMLInputElement>,
    HTMLInputElement
>;

export interface FieldPropInfo {
    tagName: 'input';
    attributes: HtmlInputProps;
}

export type FieldProps<T> = {
    [key in keyof T]: FieldPropInfo;
};
