import React, { RefObject, useMemo } from 'react';

import flow from 'lodash/fp/flow';
import upperFirst from 'lodash/fp/upperFirst';
import mapKeys from 'lodash/fp/mapKeys';
import { ReactFormFieldProps } from '../types/react-tag-props';

export type ValidationErrorRecord<T> = Partial<Record<keyof T, string[]>>;

export interface ValidationInstance<T> {
    /**
     * The JQuery Validate instance. This may be null, because
     * we can intialize the validator via the formRef until
     * after the first render. One the ref is initialized, this will
     * be set
     */
    validator?: JQueryValidation.Validator;
    showErrors: (errors: ValidationErrorRecord<T>) => void;
}

export interface UseValidationProps {
    /**
     * A reference to the form object, used to interface with JQuery in the DOM
     */
    formRef: RefObject<HTMLFormElement>;

    /**
     * additional options to pass down to Jquery Validate
     */
    validatorOptions?: JQueryValidation.ValidationOptions;
}

export const useValidation = <T extends {}>({
    formRef,
    validatorOptions,
}: UseValidationProps): ValidationInstance<T> => {
    // only reinitialize the validator if the ref changes
    const validator = useMemo(() => {
        if (formRef?.current != null) {
            $.validator.unobtrusive.parse(formRef.current);
            return $(formRef.current).validate(validatorOptions);
        }
        return undefined;
    }, [formRef?.current]);

    const showErrors = (errors: ValidationErrorRecord<T>) => {
        validator?.showErrors(
            flow(
                // mapValues(join(', ')), // convert array of errors to single string
                mapKeys(upperFirst) // the form expects TitleCase
            )(errors)
        );
    };

    return {
        validator,
        showErrors,
    };
};

/**
 * convenience function to quickly render the validation result for a given set of attributes
 */
export const validationResultFor = ({ id }: ReactFormFieldProps) => {
    if (id == null)
        throw new Error('field must have an id to bind a validator');
    return (
        <span
            className="text-danger field-validation-error"
            data-valmsg-for={id}
            data-valmsg-replace="true"
        />
    );
};
