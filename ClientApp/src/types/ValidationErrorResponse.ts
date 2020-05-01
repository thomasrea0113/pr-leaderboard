import { HttpStatusCode } from './types';
import { ValidationErrorRecord } from '../hooks/useValidation';

/**
 * object returned from the server when a validation error occurs
 */
export interface ValidationErrorResponseData<T> {
    type: string;
    status: HttpStatusCode;
    title: string;
    traceId: string;
    errors: ValidationErrorRecord<T>;
}

/**
 * Check the json response from the server to see if it indicates an error
 * @param responseData server request response data
 */
export function isValidationErrorResponseData<T = never>(
    // response from the server could be anything
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    responseData: any
): responseData is ValidationErrorResponseData<T> {
    const errorResp = responseData as ValidationErrorResponseData<T>;
    if (
        errorResp != null &&
        (typeof errorResp.type !== 'string' ||
            typeof errorResp.status !== 'number' ||
            typeof errorResp.title !== 'string' ||
            typeof errorResp.traceId !== 'string')
    )
        return false;

    // TODO also verify that errors is strictly a record of T?

    return true;
}
