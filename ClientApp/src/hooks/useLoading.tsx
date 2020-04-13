import { useState } from 'react';
import { HttpMethodsEnum } from '../types/types';
import { ErrorData, isErrorData } from '../Components/forms/Validation';
import { parseCookie } from '../Site';

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

export interface TypedResponse<T> extends Response {
    data?: T;
    errorData?: ErrorData<T>;
}

export interface UseLoadingState<D> {
    isLoading: boolean;
    isLoaded: boolean;
    response?: TypedResponse<D>;
}

export interface UseLoadingProps<D> extends UseLoadingState<D> {
    loadAsync: (props: LoadProps) => Promise<void>;
}

export interface LoadProps {
    actionUrl: string;
    actionMethod?: HttpMethodsEnum;
    additionalHeaders?: FetchHeaders;
    body?: FetchBody;
}

export const useLoading = <D extends {}>(): UseLoadingProps<D> => {
    const csrf = parseCookie().csrfToken;
    if (csrf == null) throw new Error('csrf token not found');

    const [loadingState, setState] = useState<UseLoadingState<D>>({
        isLoaded: false,
        isLoading: false,
    });
    const { isLoading } = loadingState;

    const mergeState = (newState: Partial<UseLoadingState<D>>) =>
        setState(os => ({ ...os, ...newState }));

    // if the component is not currently loading, begin loading. The wrapped
    // promise will reject immediately if we are already loading. Otherwise,
    // it completes when the inner promise completes
    const loadAsync = (props: LoadProps): Promise<void> => {
        const { actionUrl, actionMethod, additionalHeaders, body } = props;
        return new Promise<void>((resolve, reject) => {
            if (!isLoading) {
                mergeState({ isLoading: true, isLoaded: false });
                fetch(actionUrl, {
                    method: actionMethod ?? HttpMethodsEnum.GET,
                    headers: {
                        'X-CSRF-TOKEN': csrf,
                        ...additionalHeaders,
                    },
                    body,
                })
                    .then(async resp => {
                        const data = await resp.json();
                        mergeState({
                            isLoading: false,
                            isLoaded: true,
                            // Note we make no assumptions about the data here...
                            // that's the responsibility of the caller
                            response: isErrorData<D>(data)
                                ? { ...resp, errorData: data }
                                : { ...resp, data },
                        });
                    })
                    .then(
                        fullfilled => {
                            resolve(fullfilled);
                        },
                        error => {
                            mergeState({ isLoading: false });
                            return reject(error);
                        }
                    );
            } else reject(new Error('This component is already loading'));
        });
    };

    return {
        ...loadingState,
        loadAsync,
    };
};
