/**
 * The document cookie should contain the csrf token, but there's no gaurantee
 */
export interface Cookie {
    [index: string]: string;
    csrfToken: string;
}

export const parseCookie = (): Partial<Cookie> =>
    document.cookie.split(';').reduce((res, c) => {
        const [key, val] = c
            .trim()
            .split('=')
            .map(decodeURIComponent);
        try {
            return Object.assign(res, { [key]: JSON.parse(val) });
        } catch (e) {
            return Object.assign(res, { [key]: val });
        }
    }, {});

export const getCookie = (name: string) => parseCookie()[name];
