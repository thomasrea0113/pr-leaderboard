// these libraries are required outside of the react component (in the razor pages directly)
// so we import them here and include them separately on the Razor Page

// jQuery
import 'jquery';
import 'jquery-validation';
import 'bootstrap';
import './jquery.unobtrusive-ajax.extended';

// Styles
import './scss/site.scss';

import './utilities/modal-hash';

/**
 * The document cookie should contain the csrf token, but there's no gaurantee
 */
export interface Cookie {
    [index: string]: string;
    requestVerificationToken: string;
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

// store the page's scroll-y position as a data attribute on the root html element
(function trackScroll() {
    const debounce = (fn: () => void) => {
        // This holds the requestAnimationFrame reference, so we can cancel it if we wish
        let frame: number;

        // The debounce function returns a new function that can receive a variable number of arguments
        return () => {
            // If the frame variable has been defined, clear it now, and queue for next frame
            if (frame) {
                cancelAnimationFrame(frame);
            }

            // Queue our function call for the next frame
            frame = requestAnimationFrame(() => {
                // Call our function and pass any params we received
                fn();
            });
        };
    };

    // Reads out the scroll position and stores it in the data attribute
    // so we can use it in our stylesheets
    const storeScroll = () => {
        document.documentElement.dataset.scroll = window.scrollY.toString();
    };

    // Listen for new scroll events, here we debounce our `storeScroll` function
    document.addEventListener('scroll', debounce(storeScroll));

    // Update scroll position for first time
    storeScroll();
})();
