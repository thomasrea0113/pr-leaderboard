/**
 * data attributes to automatically apply a class to a target object
 */
export const attachToggleEvents = () => {
    $('[data-toggle-class][data-toggle-target]').click(({ currentTarget }) => {
        const $target = $(currentTarget);
        $($target.data('toggle-target')).toggleClass(
            $target.data('toggle-class')
        );
    });
};

// initToggle immediately
$(attachToggleEvents);
