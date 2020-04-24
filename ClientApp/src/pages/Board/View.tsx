import React, { useEffect, useRef, useState } from 'react';

// JQuery is expected to be included outside of the component, so we don't
// need to import it here to use it
// import $ from 'jquery';

import 'react-dom'; // only needed if this is to be placed directly on the page
import { ScoreTable } from '../../Components/tables/ScoreTable';
import { useLoading } from '../../hooks/useLoading';
import { User, UserView, Score, UnitOfMeasure } from '../../types/dotnet-types';
import {
    SubmitScoreForm,
    SubmitScore,
} from '../../Components/forms/SubmitScoreForm';
import { useFetchForm } from '../../hooks/useFetchForm';
import { FieldProps } from '../../Components/forms/Validation';

interface Props {
    scoresUrl: string;
    submitScoreUrl: string;
    fieldAttributes: FieldProps<SubmitScore>;
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

    const [error, setError] = useState<unknown>();

    const reloadAsync = () => loadAsync({ actionUrl: scoresUrl });

    // load on mount
    useEffect(() => {
        reloadAsync();
    }, []);

    const initial = {
        board: {
            id: undefined,
            // eslint-disable-next-line @typescript-eslint/no-object-literal-type-assertion
            uom: {
                unit: 'Kilograms',
            } as UnitOfMeasure,
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
    } = response?.data != null ? response.data : initial;

    const {
        formDispatch,
        formProps,
        fieldAttributes: fieldProps,
        SubmitButton,
        response: submitResponse,
    } = useFetchForm({
        fieldAttributes,
        formRef,
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        onSubmitError: (reason: any) => setError(reason),
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
                <form
                    {...formProps}
                    className="form-sm mb-1"
                    method="post"
                    action={submitScoreUrl}
                    ref={formRef}
                >
                    {submitResponse?.data != null ? (
                        <span className="text-primary">
                            You&apos;re score has been submitted! It will appear
                            here once it has been been reviewed by an admin.
                        </span>
                    ) : null}
                    {error != null ? (
                        <div className="text-danger mb-2">
                            Oops! Looks like something went wrong. {`${error}`}
                        </div>
                    ) : null}
                    <SubmitScoreForm fieldAttributes={fieldProps} unit={unit} />
                    <SubmitButton className="btn btn-primary">
                        Submit
                    </SubmitButton>
                </form>
            ) : null}
            <ScoreTable
                reloadAsync={reloadAsync}
                scores={scores ?? []}
                unit={unit}
            />
        </>
    );
};

export default ViewBoardComponent;
