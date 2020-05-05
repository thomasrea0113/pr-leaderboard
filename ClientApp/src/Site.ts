// these libraries are required outside of the react component (in the razor pages directly)
// so we import them here and include them separately on the Razor Page

// jQuery
import 'jquery';
import 'jquery-validation';
import 'jquery-validation-unobtrusive';
import './jquery.unobtrusive-ajax.extended';
import 'bootstrap/dist/js/bootstrap.bundle';

// Styles
import './scss/site.scss';

// Important to prevent transitions from firing on page load
$(() => $('body').removeClass('preload'));

$.fn.extend({
    isInViewport() {
        const $this = $(this);
        const elementTop = $this.offset()?.top ?? 0;
        const elementBottom = elementTop + ($this.outerHeight() ?? 0);

        const viewportTop = $(window).scrollTop() ?? 0;
        const viewportBottom = viewportTop + ($(window).height() ?? 0);

        return elementBottom > viewportTop && elementTop < viewportBottom;
    },
});

const applyVisibleClass = () => {
    $('[data-visible-class]').each((_, e) => {
        const $e = $(e);
        const visibleClass = $e.data('visible-class');
        if (!$e.hasClass(visibleClass) && $e.isInViewport())
            $e.addClass(visibleClass);
    });
};
$(applyVisibleClass);

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
        applyVisibleClass();
    };

    // Listen for new scroll events, here we debounce our `storeScroll` function
    document.addEventListener('scroll', debounce(storeScroll));

    // Update scroll position for first time
    storeScroll();
})();
