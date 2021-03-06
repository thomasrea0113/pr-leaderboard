import { HttpMethodsEnum } from '../types/types';
import { attachRenderEvents } from '../utilities/attachRenderEvents';
import { parseCookie } from '../utilities/cookie';
import {
    isValidationErrorResponseData,
    ValidationErrorResponseData,
} from '../types/ValidationErrorResponse';

export type FetchBody =
    | string
    | Blob
    | ArrayBufferView
    | ArrayBuffer
    | FormData
    | URLSearchParams
    | ReadableStream<Uint8Array>
    | null
    | undefined;

export type FetchHeaders =
    | Headers
    | string[][]
    | Record<string, string>
    | undefined;

/**
 * same props as a regular response, except the data is strongly typed
 */
export interface TypedResponse<T> extends Response {
    data: T | ValidationErrorResponseData<T>;
}

/**
 * props needed to initiate a load operation. Node, the url and method
 * are provided here, not at hook initialization. This allows
 * us to reuse the same state for many load operations.
 */
export interface LoadProps {
    actionUrl: string;
    actionMethod?: HttpMethodsEnum;
    additionalHeaders?: FetchHeaders;
    body?: FetchBody;
}

/**
 * UseLoading instance returned by the useLoading hook. Note, it exposes
 * the internal loading state as well
 */
export interface UseStatelessLoading<D> {
    loadAsync: (props: LoadProps) => Promise<TypedResponse<D>>;
}

/**
 * props needed to initialize the useLoading hook
 */
export interface UseLoadingProps<D> {
    /**
     * a guard to ensure that the data returned from the server is what
     * we said it would be
     */
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    guard?: (responseData: any) => responseData is D;
}

/**
 * a hook allowing for common loading functionality
 */
export const useStatelessLoading = <D extends {}>(
    props?: UseLoadingProps<D>
): UseStatelessLoading<D> => {
    const { guard } = { ...props };

    const csrf = parseCookie().csrfToken;
    if (csrf == null) throw new Error('csrf token not found');

    // if the component is not currently loading, begin loading. The wrapped
    // promise will reject immediately if we are already loading. Otherwise,
    // it completes when the inner promise completes
    const loadAsync = (loadProps: LoadProps): Promise<TypedResponse<D>> => {
        const { actionUrl, actionMethod, additionalHeaders, body } = loadProps;
        return new Promise<TypedResponse<D>>((resolve, reject) => {
            fetch(actionUrl, {
                method: actionMethod ?? HttpMethodsEnum.GET,
                headers: {
                    'X-CSRF-TOKEN': csrf,
                    ...additionalHeaders,
                },
                body,
            })
                .then(async resp => {
                    if (!resp.ok)
                        throw new Error(`(${resp.status}) ${resp.statusText}`);

                    const data = await resp.json();

                    // ensure that the response data was either the type we expected,
                    // or that it indicates server validation errors. If no gaurd was
                    // provided, then we assume the data is the correct type. This is
                    // dangerous!
                    const typedData =
                        guard != null && guard(data)
                            ? data
                            : isValidationErrorResponseData<D>(data)
                            ? data
                            : guard == null
                            ? (data as D)
                            : null;

                    if (typedData == null)
                        throw new Error(
                            'the data returned from the server was not of the correct type'
                        );

                    const typedResponse: TypedResponse<D> = {
                        ...resp,
                        data: typedData,
                    };

                    return typedResponse;
                })
                .then(
                    fullfilled => {
                        attachRenderEvents();
                        resolve(fullfilled);
                    },
                    error => reject(error)
                );
        });
    };

    return {
        loadAsync,
    };
};
