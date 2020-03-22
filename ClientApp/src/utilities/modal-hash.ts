import $ from 'jquery';
import 'bootstrap';

/*
 * A simple plugin that tracks the active modal on the page at loadtime
 */

const parseHash = () => new URLSearchParams(window.location.hash.substring(1));

/**
 * a function that will show/hide a modal based on the url hash
 */
export const attachHashEvents = () => {
    $('.modal').on('shown.bs.modal', ({ target }) => {
        const hash = parseHash();
        hash.set('modal', target.id);
        window.location.hash = hash.toString();
    });

    $('.modal').on('hidden.bs.modal', () => {
        const hash = parseHash();
        hash.delete('modal');
        window.location.hash = hash.toString();
    });

    const modal = parseHash().get('modal');
    if (modal != null) $(`#${modal}`).modal('show');
    else $('[data-show=true]').modal('show');
};
$(attachHashEvents);
