// these libraries are required outside of the react component (in the razor pages directly)
// so we import them here and include them separately on the Razor Page

// jQuery
import 'jquery';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import './jquery.unobtrusive-ajax.extended';
import 'bootstrap';

// Styles
import './scss/site.scss';

import { attachHashEvents } from './utilities/modal-hash';
import { attachToggleEvents } from './utilities/toggle';

// Important to prevent transitions from firing on page load
$(() => $('body').removeClass('preload'));

// after the react components load, we will need to call these functions again
export const attachJQueryEvents = () => {
    attachHashEvents();
    attachToggleEvents();
};

/**
 * converts the given form to a object. Note, this will provide a partial
 * type with the same keys at type T, however ALL of the forms elements will
 * be added (not just those on type T), so be careful! If the form contains
 * additional form inputs that don't exist on type T, then they won't be
 * available and compile time, but they will be present at runtime
 * @param form the form to convert
 */
export const formToObject = <T extends {}>(form: HTMLFormElement) =>
    Array.from(form.elements).reduce((prev, curr) => {
        const name = curr.getAttribute('name');
        const value = curr.getAttribute('value');
        if (name != null) return { ...prev, [name]: value };
        return prev;
        // the partial type makes all properties optional, so it's safe to assign an
        // empty object to a partial object of T
        // eslint-disable-next-line @typescript-eslint/no-object-literal-type-assertion
    }, {} as Partial<T>);

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
