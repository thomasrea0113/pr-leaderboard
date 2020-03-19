/**
 * Returns a promise that is gauranteed to take atleast minimumDelay miliseconds to complete
 * @param minimumDelay minimum number of miliseconds that the promise must take to resolve
 * @param promise the promise to resolve
 */
export const ensureDelay = <T>(minimumDelay: number, promise: Promise<T>) =>
    Promise.all([
        new Promise<void>(resolve => setTimeout(resolve, minimumDelay)),
        promise,
    ]);
