import React, { useEffect, useRef, useState } from 'react';

// JQuery is expected to be included outside of the component, so we don't
// need to import it here to use it
// import $ from 'jquery';

import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { useLoading } from '../../hooks/useLoading';
import { User, UserView, Score } from '../../types/dotnet-types';
import {
    SubmitScoreForm,
    SubmitScore,
    isSubmitScore,
} from '../../Components/forms/SubmitScoreForm';
import { useFetchForm } from '../../hooks/useFetchForm';
import { HttpMethodsEnum } from '../../types/types';
import { isValidationErrorResponseData } from '../../types/ValidationErrorResponse';
import { FormFieldProps } from '../../types/react-tag-props';
import { useValidation } from '../../hooks/useValidation';

interface Props {
    scoresUrl: string;
    submitScoreUrl: string;
    fieldAttributes: FormFieldProps<SubmitScore>;
}

interface State {
    user?: User;
    board: UserView;
    scores: Score[];
}

const ViewBoardComponent: React.FC<Props> = ({
    scoresUrl,
    submitScoreUrl,
    fieldAttributes,
}) => {
    const { isLoading, isLoaded, loadAsync, response } = useLoading<State>();
    const formRef = useRef<HTMLFormElement>(null);

    const [error, setError] = useState<string>();

    const validationInstance = useValidation<SubmitScore>({ formRef });

    const reloadAsync = () => loadAsync({ actionUrl: scoresUrl });

    // load on mount
    useEffect(() => {
        reloadAsync();
    }, []);

    // the intial data, to display before load or in the vent of an error.
    // just need to defined the properites so that we can spread it below
    const initial = {
        board: {
            id: undefined,
            uom: {
                unit: undefined,
            },
        },
        user: undefined,
        scores: undefined,
    };

    const {
        board: {
            id,
            uom: { unit },
        },
        user,
        scores,
    } =
        response?.data && !isValidationErrorResponseData(response.data)
            ? response.data
            : initial;

    const {
        formDispatch,
        formProps,
        fieldAttributes: fieldProps,
        SubmitButton,
        loadingProps: { response: submitResponse },
    } = useFetchForm<SubmitScore>({
        actionUrl: submitScoreUrl,
        actionMethod: HttpMethodsEnum.POST,
        fieldAttributes,
        formRef,
        guard: isSubmitScore,

        validationInstance,

        onSubmitError: (reason: Error) => setError(reason.message),
        onValidSubmit: () => {
            reloadAsync();
            formDispatch({ type: 'RESET_FORM' });
        },
    });

    useEffect(() => {
        if (isLoaded && !isLoading) {
            if (id == null) throw new Error('Server returned invalid data');

            // after we're done loading, update the form fields
            formDispatch({
                type: 'UPDATE_FIELDS',
                fields: [
                    {
                        property: 'boardId',
                        value: id,
                    },
                    {
                        property: 'userName',
                        value: user?.userName ?? '',
                    },
                ],
            });
        }
    }, [isLoaded, isLoading]);

    // TODO display loading
    return (
        <>
            <h4>Scores</h4>
            {user != null ? (
                <form {...formProps} className="form-sm mb-1" ref={formRef}>
                    {submitResponse?.data != null &&
                    !isValidationErrorResponseData(submitResponse.data) ? (
                        <span className="text-primary">
                            You&apos;re score has been submitted! It will appear
                            here once it has been been reviewed by an admin.
                        </span>
                    ) : error != null ? (
                        <div className="text-danger mb-2">
                            Oops! Looks like something went wrong. {error}
                        </div>
                    ) : null}
                    <SubmitScoreForm
                        fieldAttributes={fieldProps}
                        unit={unit ?? 'Kilograms'}
                    />
                    <SubmitButton className="btn btn-primary">
                        Submit
                    </SubmitButton>
                </form>
            ) : null}
            <ScoreTable
                reloadAsync={reloadAsync}
                scores={scores ?? []}
                unit={unit ?? 'Kilograms'}
            />
        </>
    );
};

export default ViewBoardComponent;
