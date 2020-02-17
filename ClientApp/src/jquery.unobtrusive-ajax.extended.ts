/* eslint-disable eqeqeq */
/* eslint-disable no-param-reassign */
/* eslint-disable prefer-rest-params */
/* eslint-disable no-alert */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prefer-const */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/camelcase */
/* eslint-disable func-names */
/* eslint-disable @typescript-eslint/explicit-function-return-type */

// A quick and dirty port of https://github.com/aspnet/jquery-ajax-unobtrusive to typescript,
// so that I can provide some custom functionality.

// Unobtrusive Ajax support library for jQuery
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// @version <placeholder>
//
// Microsoft grants you the right to use these script files for the sole
// purpose of either: (i) interacting through your browser with the Microsoft
// website or online service, subject to the applicable licensing or use
// terms; or (ii) using the files as included with a Microsoft product subject
// to that product's license terms. Microsoft reserves all other rights to the
// files not expressly granted by Microsoft, whether by implication, estoppel
// or otherwise. Insofar as a script file is dual licensed under GPL,
// Microsoft neither took the code under GPL nor distributes it thereunder but
// under the terms set out in this paragraph. All notices and licenses
// below are for informational purposes only.

/* jslint white: true, browser: true, onevar: true, undef: true, nomen: true, eqeqeq: true, plusplus: true, bitwise: true, regexp: true, newcap: true, immed: true, strict: false */
/* global window: false, jQuery: false */

export {};

(function($) {
    // region my-additions
    const enableForm = (
        element: JQuery<HTMLElement>,
        enable: boolean
    ): void => {
        if (element.is('[data-ajax-disable-on-load=true]')) {
            const inputs = element.find('input,button[type=submit],select');
            if (enable) inputs.removeAttr('disabled');
            else inputs.attr('disabled', '');
        }
    };

    const ajaxFormSelector = '[data-ajax=true]';
    const ajaxFormInitialSelector = `${ajaxFormSelector}[data-ajax-initial-url]`;

    $(document).ready((): void => {
        // when the page is ready, begin loading each form that has initial data
        $(ajaxFormInitialSelector).each(function loadInitial(): void {
            const loading = this.getAttribute('data-ajax-loading');
            const url = String(this.getAttribute('data-ajax-initial-url'));
            const $this = $(this);

            enableForm($this, false);

            const loadingElement: JQuery<HTMLElement> | null =
                loading != null ? $(loading) : null;

            loadingElement?.show();
            $this.load(url, function callback(): void {
                enableForm($this, true);
                loadingElement?.hide();
            });
        });
    });
    // end region

    const data_click = 'unobtrusiveAjaxClick';
    const data_target = 'unobtrusiveAjaxClickTarget';
    const data_validation = 'unobtrusiveValidation';

    function getFunction(code: any, argNames: any) {
        let fn: any = window;
        const parts = (code || '').split('.');
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
        if (typeof fn === 'function') {
            return fn;
        }
        argNames.push(code);
        return Function.constructor.apply(null, argNames);
    }

    function isMethodProxySafe(method: any) {
        return method === 'GET' || method === 'POST';
    }

    function asyncOnBeforeSend(xhr: any, method: any) {
        if (!isMethodProxySafe(method)) {
            xhr.setRequestHeader('X-HTTP-Method-Override', method);
        }
    }

    function asyncOnSuccess(element: any, data: any, contentType: any) {
        let mode: any;

        if (contentType.indexOf('application/x-javascript') !== -1) {
            // jQuery already executes JavaScript for us
            return;
        }

        mode = (element.getAttribute('data-ajax-mode') || '').toUpperCase();
        $(element.getAttribute('data-ajax-update')).each(function(i, update) {
            let top;

            switch (mode) {
                case 'BEFORE':
                    $(update).prepend(data);
                    break;
                case 'AFTER':
                    $(update).append(data);
                    break;
                case 'REPLACE-WITH':
                    $(update).replaceWith(data);
                    break;
                default:
                    $(update).html(data);
                    break;
            }
        });
    }

    function asyncRequest(element: any, options: any) {
        let confirm: any;
        let loading: any;
        let method: any;
        let duration: any;

        confirm = element.getAttribute('data-ajax-confirm');
        if (confirm && !window.confirm(confirm)) {
            return;
        }

        loading = $(element.getAttribute('data-ajax-loading'));
        duration =
            parseInt(element.getAttribute('data-ajax-loading-duration'), 10) ||
            0;

        $.extend(options, {
            type: element.getAttribute('data-ajax-method') || undefined,
            url: element.getAttribute('data-ajax-url') || undefined,
            cache:
                (
                    element.getAttribute('data-ajax-cache') || ''
                ).toLowerCase() === 'true',
            beforeSend(xhr: any) {
                let result;
                asyncOnBeforeSend(xhr, method);
                result = getFunction(element.getAttribute('data-ajax-begin'), [
                    'xhr',
                ]).apply(element, arguments);
                if (result !== false) {
                    loading.show(duration);
                    enableForm($(element), false);
                }
                return result;
            },
            complete(xhr: JQuery.jqXHR<any>) {
                loading.hide(duration);
                enableForm($(element), true);
                getFunction(element.getAttribute('data-ajax-complete'), [
                    'xhr',
                    'status',
                ]).apply(element, arguments);
            },
            success(data: any, status: any, xhr: any) {
                asyncOnSuccess(
                    element,
                    data,
                    xhr.getResponseHeader('Content-Type') || 'text/html'
                );
                getFunction(element.getAttribute('data-ajax-success'), [
                    'data',
                    'status',
                    'xhr',
                ]).apply(element, arguments);
            },
            error() {
                getFunction(element.getAttribute('data-ajax-failure'), [
                    'xhr',
                    'status',
                    'error',
                ]).apply(element, arguments);
            },
        });

        options.data.push({
            name: 'X-Requested-With',
            value: 'XMLHttpRequest',
        });

        method = options.type.toUpperCase();
        if (!isMethodProxySafe(method)) {
            options.type = 'POST';
            options.data.push({
                name: 'X-HTTP-Method-Override',
                value: method,
            });
        }

        // change here:
        // Check for a Form POST with enctype=multipart/form-data
        // add the input file that were not previously included in the serializeArray()
        // set processData and contentType to false
        const $element = $(element);
        if (
            $element.is('form') &&
            $element.attr('enctype') == 'multipart/form-data'
        ) {
            const formdata = new FormData();
            $.each(options.data, function(i, v) {
                formdata.append(v.name, v.value);
            });
            $('input[type=file]', $element).each(function() {
                const file: any = this;
                $.each(file.files, function(n, v) {
                    formdata.append(file.name, v);
                });
            });
            $.extend(options, {
                processData: false,
                contentType: false,
                data: formdata,
            });
        }
        // end change

        $.ajax(options);
    }

    function validate(form: any) {
        const validationInfo = $(form).data(data_validation);
        return (
            !validationInfo ||
            !validationInfo.validate ||
            validationInfo.validate()
        );
    }

    $(document).on('click', 'a[data-ajax=true]', function(evt) {
        evt.preventDefault();
        asyncRequest(this, {
            url: this.href,
            type: 'GET',
            data: [],
        });
    });

    $(document).on('click', 'form[data-ajax=true] input[type=image]', function(
        evt
    ) {
        const { name } = evt.target;
        const target = $(evt.target);
        const form = $(target.parents('form')[0]);
        const offset: any = target.offset();

        form.data(data_click, [
            { name: `${name}.x`, value: Math.round(evt.pageX - offset.left) },
            { name: `${name}.y`, value: Math.round(evt.pageY - offset.top) },
        ]);

        setTimeout(function() {
            form.removeData(data_click);
        }, 0);
    });

    $(document).on('click', 'form[data-ajax=true] :submit', function(evt) {
        const { name } = evt.currentTarget;
        const target = $(evt.target);
        const form = $(target.parents('form')[0]);

        form.data(
            data_click,
            name ? [{ name, value: evt.currentTarget.value }] : []
        );
        form.data(data_target, target);

        setTimeout(function() {
            form.removeData(data_click);
            form.removeData(data_target);
        }, 0);
    });

    $(document).on('submit', 'form[data-ajax=true]', function(evt) {
        const clickInfo = $(this).data(data_click) || [];
        const clickTarget = $(this).data(data_target);
        const isCancel =
            clickTarget &&
            (clickTarget.hasClass('cancel') ||
                clickTarget.attr('formnovalidate') !== undefined);
        evt.preventDefault();
        if (!isCancel && !validate(this)) {
            return;
        }
        asyncRequest(this, {
            url: this.action,
            type: this.method || 'GET',
            data: clickInfo.concat($(this).serializeArray()),
        });
    });
})(jQuery);
