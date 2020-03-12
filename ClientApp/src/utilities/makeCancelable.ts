export class CancelError extends Error {
    public constructor(message: string = 'Promise Canceled') {
        super(message);
    }
}

export interface CancelablePromise<T> {
    promise: Promise<T | void>;
    cancel: () => void;
}

export const makeCancelable = <T>(
    promise: Promise<T | void>
): CancelablePromise<T> => {
    let hasCanceled = false;

    const Canceled = new CancelError();

    const wrappedPromise = new Promise<void | T>((resolve, reject) => {
        promise.then(
            val => {
                if (hasCanceled) {
                    reject(Canceled);
                } else resolve(val);
                return val;
            },
            error => (hasCanceled ? reject(Canceled) : reject(error))
        );
    });

    return {
        promise: wrappedPromise,
        cancel: () => {
            hasCanceled = true;
        },
    };
};
