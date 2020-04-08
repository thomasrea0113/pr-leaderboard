import flow from 'lodash/fp/flow';
import fromPairs from 'lodash/fp/fromPairs';
import map from 'lodash/fp/map';
import toPairs from 'lodash/fp/toPairs';
import mapValues from 'lodash/fp/mapValues';

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

/**
 * contains information about the type as it will appear in a form
 */
export type FieldProps<T> = {
    [key in keyof T]: FieldPropInfo;
};

/**
 * a convenient function to easily merge in additional html input properties
 * @param props the attributes for the form type
 * @param attributesCallback a callback to generate appended attributes
 */
export const mergeAttributes = <T extends {}>(
    props: FieldProps<T>,
    attributesCallback: (a: [keyof T, FieldPropInfo]) => Partial<HtmlInputProps>
) =>
    flow(
        toPairs,
        map<[keyof T, FieldPropInfo], [keyof T, FieldPropInfo]>(kv => [
            kv[0],
            {
                ...kv[1],
                attributes: {
                    ...kv[1].attributes,
                    ...attributesCallback(kv),
                },
            },
        ]),
        fromPairs
    )(props) as FieldProps<T>;

/**
 * defines a type that can be mapped to a the elements of a form
 */
export type FormValues<T> = Partial<
    Record<keyof T, string | number | string[] | undefined>
>;

export const fieldValues = <T>(props: FieldProps<T>) =>
    mapValues((v: FieldPropInfo) => v.attributes.value, props);
