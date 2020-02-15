import $ from 'jquery';

$(document).ready((): void => {
    $('[data-ajax-initial-url]').each(function each(): void {
        const loading = this.getAttribute('data-ajax-loading');
        const url = String(this.getAttribute('data-ajax-initial-url'));
        if (loading != null) {
            const loadingElement = $(loading);
            loadingElement.show();
            $(this).load(url, (): JQuery => loadingElement.hide());
        } else {
            $(this).load(url);
        }
    });
});
