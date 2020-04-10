import flow from 'lodash/fp/flow';
import fromPairs from 'lodash/fp/fromPairs';
import map from 'lodash/fp/map';
import toPairs from 'lodash/fp/toPairs';
import mapValues from 'lodash/fp/mapValues';
import { HttpStatusCode } from '../../types/types';

export type ErrorRecord<T> = Record<keyof T, string[]>;

export interface ErrorData<T> {
    type: string;
    status: HttpStatusCode;
    title: string;
    traceId: string;
    errors: ErrorRecord<T>;
}

export function isErrorData<T>(
    responseData: ErrorData<T> | T
): responseData is ErrorData<T> {
    const errorResp = responseData as ErrorData<T>;
    if (
        typeof errorResp.type !== 'string' ||
        typeof errorResp.status !== 'number' ||
        typeof errorResp.title !== 'string' ||
        typeof errorResp.traceId !== 'string'
    )
        return false;

    // TODO also verify that errors is strictly a record of T?

    return true;
}

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
    errors?: string[];
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
